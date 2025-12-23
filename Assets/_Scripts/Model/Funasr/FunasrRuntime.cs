using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FunasrRuntime : MonoSingleton<FunasrRuntime>
{
    public Text txtContent;
    #region 配置参数
    [Header("服务器配置")]
    private string host = "120.55.71.69";
    // private string host = "127.0.0.1";
    private string port = "10096";
    // private string port = "11095";

    [Header("音频配置")]
    public int waveBufferMilliseconds = 600;
    public int waveBitsPerSample = 16;
    public int waveChannels = 1;
    public int waveSampleRate = 16000;

    [Header("麦克风设置")]
    [SerializeField] private string selectedMicrophoneDevice = null; // 当前选择的麦克风设备，null表示默认设备
    [SerializeField] private bool showMicrophoneSelection = false; // 是否在运行时显示麦克风设备选择界面

    [Header("识别配置")]
    public string onlineAsrMode = string.Empty;
    #endregion

    #region 内部变量
    private UnityWebSocketClient websocketClient;
    private UnityWaveCollect waveCollect = new UnityWaveCollect();
    public static readonly ConcurrentQueue<byte[]> ActiveAudioSet = new ConcurrentQueue<byte[]>();
    public static readonly ConcurrentQueue<string> AudioFileQueue = new ConcurrentQueue<string>();

    // 识别状态和结果
    private string recognitionResult = string.Empty;
    public bool isRecognizing = false;
    public bool isConnected = false;

    // 线程安全的消息队列，用于将WebSocket消息从后台线程传递到主线程
    private readonly ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
    #endregion

    #region Unity回调
    void Start()
    {
        // 初始化时传入this实例
        websocketClient = new UnityWebSocketClient(this);
        InitializeAudioSettings();
    }

    void Update()
    {

        // 处理音频队列，限制每帧处理的文件数量
        if (AudioFileQueue.Count > 0)
        {
            ProcessAudioQueue();
        }

        // 处理WebSocket消息队列，限制每帧处理的消息数量
        int messageCount = 0;
        int maxMessagesPerFrame = 10; // 每帧最多处理10条消息
        while (!messageQueue.IsEmpty && messageCount < maxMessagesPerFrame)
        {
            string message;
            if (messageQueue.TryDequeue(out message))
            {
                // 检查是否是错误消息
                if (message.StartsWith("ERROR:"))
                {
                    string errorMsg = message.Substring(6); // 移除"ERROR:"前缀
                    Debug.LogError($"接收消息错误: {errorMsg}");
                    FunasrEvents.TriggerServerError("WebSocket错误: " + errorMsg);
                }
                else
                {
                    ProcessWebSocketMessage(message);
                }
                messageCount++;
            }
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            // 先停止识别，然后关闭连接
            StopRealtimeRecognition();

            // 清空所有队列
            while (ActiveAudioSet.TryDequeue(out _)) { }
            while (AudioFileQueue.TryDequeue(out _)) { }
            while (messageQueue.TryDequeue(out _)) { }
            while (UnityWaveCollect.voiceBuff.TryDequeue(out _)) { }

            // 关闭WebSocket连接
            Debug.Log("应用退出，清理WebSocket资源");
            websocketClient?.CloseConnection();
        }
        catch (Exception ex)
        {
            Debug.LogError($"应用退出时清理资源出错: {ex.Message}");
        }
    }



    private void OnDisable()
    {
        try
        {
            // 当组件禁用时，确保停止识别并关闭连接
            if (isRecognizing)
            {
                StopRealtimeRecognition();
            }

            // 清空所有队列
            while (ActiveAudioSet.TryDequeue(out _)) { }
            while (AudioFileQueue.TryDequeue(out _)) { }
            while (messageQueue.TryDequeue(out _)) { }
            while (UnityWaveCollect.voiceBuff.TryDequeue(out _)) { }

            // 关闭WebSocket连接
            Debug.Log("组件禁用，清理WebSocket资源");
            websocketClient?.CloseConnection();
        }
        catch (Exception ex)
        {
            Debug.LogError($"组件禁用时清理资源出错: {ex.Message}");
        }
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 连接到FunASR服务器
    /// </summary>
    /// <returns>连接状态</returns>
    public async Task<string> ConnectToServer()
    {
        try
        {
            // 设置服务器信息
            websocketClient.SetServerInfo(host, port);

            // 添加连接超时机制
            var connectTask = websocketClient.Connect();
            if (await Task.WhenAny(connectTask, Task.Delay(5000)) != connectTask)
            {
                // 连接超时
                isConnected = false;
                string errorMessage = "连接超时，请检查服务器地址和端口";
                FunasrEvents.TriggerConnectionStatus(isConnected, errorMessage);
                return errorMessage;
            }

            string status = await connectTask;
            isConnected = status.Contains("成功");
            FunasrEvents.TriggerConnectionStatus(isConnected, status);

            // 连接后稍等一下，确保连接稳定
            await Task.Delay(500);

            // 再次检查连接状态
            if (!websocketClient.IsConnected())
            {
                isConnected = false;
                string errorMessage = "连接成功后无法保持，服务器可能已断开连接";
                FunasrEvents.TriggerConnectionStatus(isConnected, errorMessage);
                return errorMessage;
            }

            Debug.Log($"服务器连接状态: {(isConnected ? "已连接" : "未连接")}");
            return status;
        }
        catch (Exception ex)
        {
            // 处理连接异常
            isConnected = false;
            string errorMessage = $"连接出错: {ex.Message}";
            FunasrEvents.TriggerConnectionStatus(isConnected, errorMessage);
            return errorMessage;
        }
    }

    /// <summary>
    /// 开始实时语音识别
    /// </summary>
    /// <param name="mode">识别模式: "online"或"2pass"</param>
    public void StartRealtimeRecognition(string mode)
    {

        if (!isConnected)
        {
            Debug.LogError("未连接到服务器，请先调用ConnectToServer");
            return;
        }
        if (!websocketClient.IsConnected())
        {
            Debug.LogError("WebSocket连接已断开，请重新连接服务器");
            isConnected = false;
            FunasrEvents.TriggerConnectionStatus(false, "WebSocket连接已断开");
            return;
        }

        if (isRecognizing)
        {
            Debug.LogWarning("已经在进行识别，请先停止当前识别");
            return;
        }

        try
        {
            onlineAsrMode = mode == "1" ? "online" : "2pass";
            Debug.Log($"开始实时识别，模式: {onlineAsrMode}");

            waveCollect.StartRecording();
            websocketClient.StartOnlineRecognition(onlineAsrMode);
            isRecognizing = true;

            StartCoroutine(ProcessAudioCoroutine());
        }
        catch (Exception ex)
        {
            Debug.LogError($"开始识别失败: {ex.Message}");
            waveCollect.StopRecording();
            isRecognizing = false;
        }
    }

    /// <summary>
    /// 停止实时语音识别
    /// </summary>
    public void StopRealtimeRecognition()
    {
        if (!isRecognizing)
            return;

        try
        {
            // 先停止录音
            waveCollect.StopRecording();

            // 清空音频缓冲区
            while (UnityWaveCollect.voiceBuff.TryDequeue(out _)) { }
            while (ActiveAudioSet.TryDequeue(out _)) { }

            // 发送结束信号但不等待
            websocketClient?.EndOnlineRecognition(false);

            isRecognizing = false;
            isConnected = false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"停止识别出错: {ex.Message}");
        }
    }

    /// <summary>
    /// 转写音频文件
    /// </summary>
    /// <param name="filePath">音频文件路径</param>
    public void TranscribeAudioFile(string filePath)
    {
        if (!isConnected)
        {
            Debug.LogError("未连接到服务器，请先调用ConnectToServer");
            return;
        }

        AudioFileQueue.Enqueue(filePath);
    }

    /// <summary>
    /// 获取最新的识别结果
    /// </summary>
    /// <returns>识别结果文本</returns>
    public string GetRecognitionResult()
    {
        return recognitionResult;
    }

    /// <summary>
    /// 设置识别结果回调
    /// </summary>
    /// <param name="callback">回调函数</param>
    public void SetResultCallback(Action<string> callback)
    {
        FunasrEvents.OnRecognitionResult += callback;
    }

    /// <summary>
    /// 设置临时结果回调（仅在2pass模式下有效）
    /// </summary>
    /// <param name="callback">回调函数</param>
    public void SetInterimResultCallback(Action<string> callback)
    {
        FunasrEvents.OnInterimResult += callback;
    }

    /// <summary>
    /// 设置连接状态回调
    /// </summary>
    /// <param name="callback">回调函数</param>
    public void SetConnectionStatusCallback(Action<bool, string> callback)
    {
        FunasrEvents.OnConnectionStatus += callback;
    }

    /// <summary>
    /// 获取可用的麦克风设备列表
    /// </summary>
    /// <returns>麦克风设备名称数组</returns>
    public string[] GetMicrophoneDevices()
    {
        return Microphone.devices;
    }

    /// <summary>
    /// 获取当前选择的麦克风设备
    /// </summary>
    /// <returns>当前麦克风设备名称，null表示默认设备</returns>
    public string GetSelectedMicrophoneDevice()
    {
        return selectedMicrophoneDevice;
    }

    /// <summary>
    /// 设置要使用的麦克风设备
    /// </summary>
    /// <param name="deviceName">麦克风设备名称，null表示使用默认设备</param>
    public void SetMicrophoneDevice(string deviceName)
    {
        // 如果正在识别，不允许切换设备
        if (isRecognizing)
        {
            Debug.LogWarning("正在进行识别，无法切换麦克风设备");
            return;
        }

        // 检查设备是否存在
        if (deviceName != null && !Microphone.devices.Contains(deviceName))
        {
            Debug.LogError($"麦克风设备 '{deviceName}' 不存在");
            return;
        }

        selectedMicrophoneDevice = deviceName;
        Debug.Log($"已设置麦克风设备: {(deviceName == null ? "默认设备" : deviceName)}");
    }

    /// <summary>
    /// 清除在线识别缓冲区
    /// </summary>
    public void ClearOnlineBuffer()
    {
        if (websocketClient != null)
        {
            websocketClient.SetOnlineBuff(string.Empty);
            Debug.Log("已清除在线识别缓冲区");
        }
    }

    /// <summary>
    /// 设置是否显示麦克风选择界面
    /// </summary>
    /// <param name="show">是否显示</param>
    public void ShowMicrophoneSelectionGUI(bool show)
    {
        showMicrophoneSelection = show;
    }

    /// <summary>
    /// 显示麦克风设备选择界面
    /// </summary>
    private void DisplayMicrophoneSelectionGUI()
    {
        // 获取所有麦克风设备
        string[] devices = Microphone.devices;

        // 如果没有麦克风设备，显示提示
        if (devices.Length == 0)
        {
            GUI.Box(new Rect(10, 10, 300, 50), "没有找到麦克风设备");
            return;
        }

        // 限制窗口大小，避免设备过多时GUI过大
        int height = Mathf.Min(60 + devices.Length * 30, Screen.height - 20);

        // 创建一个窗口来显示麦克风设备列表
        GUI.Box(new Rect(10, 10, 300, height), "选择麦克风设备");

        // 添加滚动视图以便在设备过多时可以滚动
        scrollPosition = GUI.BeginScrollView(
            new Rect(10, 40, 280, height - 50),
            scrollPosition,
            new Rect(0, 0, 260, devices.Length * 30 + 40));

        // 添加默认麦克风选项
        bool isDefaultSelected = selectedMicrophoneDevice == null;
        bool newDefaultSelection = GUI.Toggle(new Rect(10, 10, 260, 20), isDefaultSelected, "默认设备");
        if (newDefaultSelection && !isDefaultSelected)
        {
            SetMicrophoneDevice(null);
        }

        // 添加每个设备的选项
        for (int i = 0; i < devices.Length; i++)
        {
            bool isSelected = selectedMicrophoneDevice == devices[i];
            bool newSelection = GUI.Toggle(new Rect(10, 40 + i * 30, 260, 20), isSelected, devices[i]);

            if (newSelection && !isSelected)
            {
                SetMicrophoneDevice(devices[i]);
            }
        }

        GUI.EndScrollView();

        // 添加关闭按钮
        if (GUI.Button(new Rect(200, 10, 100, 20), "关闭"))
        {
            showMicrophoneSelection = false;
        }
    }
    #endregion

    #region 内部方法
    private void InitializeAudioSettings()
    {
        waveCollect.SetAudioSettings(waveSampleRate, waveBitsPerSample, waveChannels, waveBufferMilliseconds, selectedMicrophoneDevice);
        // 设置全局采样率变量
        UnityWebSocketClient.waveSampleRate = waveSampleRate;
    }

    private void ProcessAudioQueue()
    {
        // 每帧只处理一个文件，避免长时间阻塞主线程
        string filePath;
        if (AudioFileQueue.TryDequeue(out filePath) && !string.IsNullOrEmpty(filePath))
        {
            // 在后台线程处理文件发送
            Task.Run(() => websocketClient.SendAudioFile(filePath));
        }
    }

    private IEnumerator ProcessAudioCoroutine()
    {
        // 在协程开始时检查连接状态
        if (!websocketClient.IsConnected())
        {
            Debug.LogError("音频处理协程启动时WebSocket未连接");
            isRecognizing = false;
            yield break;
        }
        float timeSinceLastProcess = 0f;
        float processInterval = 0.01f;
        float timeSinceLastCheck = 0f;
        float checkInterval = 1.0f;

        while (isRecognizing)
        {
            try
            {
                timeSinceLastProcess += Time.deltaTime;
                timeSinceLastCheck += Time.deltaTime;

                if (timeSinceLastCheck >= checkInterval)
                {
                    timeSinceLastCheck = 0f;
                    if (!websocketClient.IsConnected())
                    {
                        Debug.LogError("音频处理过程中WebSocket连接已断开");
                        // isRecognizing = false;
                        // isConnected = false;
                        FunasrEvents.TriggerConnectionStatus(false, "WebSocket连接已断开");
                        continue;
                    }
                }

                if (timeSinceLastProcess >= processInterval)
                {
                    timeSinceLastProcess = 0f;

                    int packetCount = 0;
                    int maxPacketsPerBatch = 5;

                    while (!UnityWaveCollect.voiceBuff.IsEmpty && packetCount < maxPacketsPerBatch)
                    {
                        byte[] buff;
                        if (UnityWaveCollect.voiceBuff.TryDequeue(out buff) && buff != null)
                        {
                            try
                            {
                                ActiveAudioSet.Enqueue(buff);
                                byte[] audioData = new byte[buff.Length];
                                Buffer.BlockCopy(buff, 0, audioData, 0, buff.Length);

                                if (!websocketClient.IsConnected())
                                {
                                    Debug.LogError("发送音频前WebSocket连接已断开");
                                    // isRecognizing = false;
                                    // isConnected = false;
                                    FunasrEvents.TriggerConnectionStatus(false, "WebSocket连接已断开");
                                    yield break;
                                }

                                Task.Run(() => websocketClient.SendAudioData(audioData));
                                packetCount++;
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"处理音频数据包时出错: {ex.Message}\nStackTrace: {ex.StackTrace}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"音频处理协程出错: {ex.Message}\nStackTrace: {ex.StackTrace}");
                // isRecognizing = false;
                break;
            }

            yield return null;
        }
    }



    private void ProcessWebSocketMessages()
    {
        // 处理从WebSocket接收到的消息
        while (!messageQueue.IsEmpty)
        {
            string message;
            if (messageQueue.TryDequeue(out message))
            {
                // 检查是否是错误消息
                if (message.StartsWith("ERROR:"))
                {
                    string errorMsg = message.Substring(6); // 移除"ERROR:"前缀
                    Debug.LogError($"接收消息错误: {errorMsg}");
                    FunasrEvents.TriggerServerError("WebSocket错误: " + errorMsg);
                    continue;
                }

                ProcessWebSocketMessage(message);
            }
        }
    }

    public void ClearRecBuff()
    {
        if (websocketClient != null)
        {
            // websocketClient.SetOnlineBuff(string.Empty);
            websocketClient.SetRecBuff(string.Empty);
            Debug.Log("已清除在线识别缓冲区");
        }
    }
    // 在主线程中处理WebSocket消息
    private void ProcessWebSocketMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        try
        {
            // 手动解析JSON
            Dictionary<string, object> jsonData = websocketClient.ParseJson(message);

            if (jsonData.TryGetValue("mode", out object modeObj) &&
                jsonData.TryGetValue("text", out object textObj) &&
                jsonData.TryGetValue("is_final", out object isFinalObj))
            {
                string mode = modeObj.ToString();
                string text = textObj.ToString();
                bool isFinal = bool.Parse(isFinalObj.ToString());

                // 处理识别结果
                if (mode == "2pass-online")
                {
                    string onlineBuff = websocketClient.GetOnlineBuff();
                    onlineBuff += text;

                    websocketClient.SetOnlineBuff(onlineBuff);
                    recognitionResult = websocketClient.GetRecBuff() + onlineBuff;
                    Debug.Log("实时识别（临时）: " + recognitionResult);
                    if (txtContent!= null)
                        txtContent.text = recognitionResult + "...";
                    FunasrEvents.TriggerInterimResult(recognitionResult);

                }
                else if (mode == "2pass-offline")
                {
                    string recBuff = websocketClient.GetRecBuff();
                    recBuff += text;
                    websocketClient.SetRecBuff(recBuff);
                    websocketClient.SetOnlineBuff(string.Empty);
                    recognitionResult = recBuff;
                    Debug.Log("实时识别（确认）: " + recognitionResult);
                    if (txtContent!= null)
                        txtContent.text = recognitionResult;
                    FunasrEvents.TriggerRecognitionResult(recognitionResult);
                    if (recBuff.Length > 30)
                    {
                        ClearRecBuff();
                    }
                }
                else if (mode == "offline" || mode == "online")
                {
                    // 离线转写或单遍在线识别
                    // 对于单遍模式，直接添加到最终结果
                    recognitionResult += text;
                    Debug.Log("识别结果: " + recognitionResult);
                    FunasrEvents.TriggerRecognitionResult(recognitionResult);
                }

                // 如果是最终结果，清空累积缓冲区
                if (isFinal && onlineAsrMode != "offline")
                {
                    websocketClient.SetRecBuff(string.Empty);
                    FunasrEvents.TriggerRecognitionEnd(true);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("解析JSON错误: " + ex.Message);
            FunasrEvents.TriggerServerError("解析响应出错: " + ex.Message);
        }
    }
    #endregion

    #region 内部实现类
    /// <summary>
    /// Unity适配的音频采集类
    /// </summary>
    private class UnityWaveCollect
    {
        private AudioClip recordingClip;
        private bool isRecording = false;
        private int sampleRate = 16000;
        private int bitsPerSample = 16;
        private int channels = 1;
        private int bufferMilliseconds = 600;
        private string microphoneDevice = null; // null表示默认设备

        public static readonly ConcurrentQueue<byte[]> voiceBuff = new ConcurrentQueue<byte[]>();


        public void SetAudioSettings(int sampleRate, int bitsPerSample, int channels, int bufferMilliseconds, string microphoneDevice)
        {
            this.sampleRate = sampleRate;
            this.bitsPerSample = bitsPerSample;
            this.channels = channels;
            this.bufferMilliseconds = bufferMilliseconds;
            this.microphoneDevice = microphoneDevice;
        }

        public void StartRecording()
        {
            // 清空缓存数据
            while (voiceBuff.TryDequeue(out _)) { }

            if (recordingClip != null)
            {
                UnityEngine.Object.Destroy(recordingClip);
            }

            isRecording = true;

            // 检查麦克风设备是否存在
            string deviceToUse = microphoneDevice;
            if (deviceToUse != null && !Microphone.devices.Contains(deviceToUse))
            {
                Debug.LogWarning($"麦克风设备 '{deviceToUse}' 不存在，将使用默认设备");
                deviceToUse = null;
            }

            // 使用配置的麦克风设备
            recordingClip = Microphone.Start(deviceToUse, true, 10, sampleRate);

            // 启动采集协程
            MonoBehaviour host = GameObject.FindObjectOfType<FunasrRuntime>();
            host.StartCoroutine(RecordingCoroutine());
        }

        public void StopRecording()
        {
            isRecording = false;

            // 检查麦克风设备是否存在
            string deviceToUse = microphoneDevice;
            if (deviceToUse != null && !Microphone.devices.Contains(deviceToUse))
            {
                deviceToUse = null;
            }

            if (Microphone.IsRecording(deviceToUse))
            {
                Microphone.End(deviceToUse);
            }

            if (recordingClip != null)
            {
                UnityEngine.Object.Destroy(recordingClip);
                recordingClip = null;
            }
        }

        private IEnumerator RecordingCoroutine()
        {
            int lastPosition = 0;
            int bufferSize = sampleRate * bufferMilliseconds / 1000;
            float[] samples = new float[bufferSize];

            // 获取要使用的麦克风设备
            string deviceToUse = microphoneDevice;
            if (deviceToUse != null && !Microphone.devices.Contains(deviceToUse))
            {
                Debug.LogWarning($"麦克风设备 '{deviceToUse}' 不存在，将使用默认设备");
                deviceToUse = null;
            }

            // 添加一个安全计数器，防止死循环
            int loopCount = 0;
            int maxLoops = 10000000; // 设置一个合理的上限

            while (isRecording && recordingClip != null && loopCount < maxLoops)
            {
                loopCount++;

                int currentPosition = Microphone.GetPosition(deviceToUse);
                if (currentPosition < 0)
                {
                    // 麦克风可能出现问题
                    Debug.LogWarning("麦克风位置获取失败，将停止录音");
                    isRecording = false;
                    break;
                }

                if (currentPosition < lastPosition)
                {
                    // 录音重置了，跳过这一帧
                    lastPosition = currentPosition;
                    yield return null;
                    continue;
                }

                if (currentPosition > lastPosition + bufferSize)
                {
                    try
                    {
                        recordingClip.GetData(samples, lastPosition);
                        byte[] audioData = ConvertFloatToPCM(samples);
                        voiceBuff.Enqueue(audioData);

                        lastPosition += bufferSize;
                        // 重置循环计数器
                        loopCount = 0;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"音频数据处理错误: {ex.Message}");
                    }
                }

                yield return null;
            }

            if (loopCount >= maxLoops)
            {
                Debug.LogError("录音协程可能陷入死循环，已强制停止");
            }
        }

        private byte[] ConvertFloatToPCM(float[] samples)
        {
            byte[] pcmData = new byte[samples.Length * 2]; // 16位采样需要2字节
            int rescaleFactor = 32767; // 对于16位采样

            for (int i = 0; i < samples.Length; i++)
            {
                short value = (short)(samples[i] * rescaleFactor);
                pcmData[i * 2] = (byte)(value & 0xff);
                pcmData[i * 2 + 1] = (byte)((value >> 8) & 0xff);
            }

            return pcmData;
        }
    }

    /// <summary>
    /// Unity适配的WebSocket客户端
    /// </summary>
    private class UnityWebSocketClient
    {
        private ClientWebSocket webSocket;
        private string serverHost;
        private string serverPort;
        private bool isRunning = false;
        private string recognitionResult = string.Empty;
        private string onlineBuff = string.Empty; // 在线识别的缓冲
        private string recBuff = string.Empty; // 最终识别结果的缓冲

        // 引用FunasrRuntime实例，避免在非主线程中使用FindObjectOfType
        private FunasrRuntime runtimeInstance;

        // 添加取消令牌源，用于取消正在进行的WebSocket操作
        private CancellationTokenSource cancellationTokenSource;

        // 添加锁对象，用于同步接收操作
        private readonly object receiveLock = new object();
        private bool isReceiving = false;

        private int[] chunkSizeArray = new int[] { 5, 10, 5 };
        private int chunkInterval = 10;

        // 静态采样率变量，用于音频块大小计算
        public static int waveSampleRate = 16000;

        // 接收消息的任务
        private Task receiveTask;

        private int reconnectAttempts = 0;
        private const int MaxReconnectAttempts = 3;
        private const int ReconnectDelayMs = 2000;

        // 构造函数，接收FunasrRuntime实例
        public UnityWebSocketClient(FunasrRuntime runtime)
        {
            this.runtimeInstance = runtime;
            this.cancellationTokenSource = new CancellationTokenSource();
        }
        public event Action<bool> OnConnectionStatusChanged;
        // 提供对内部缓冲区的访问方法
        public string GetOnlineBuff() { return onlineBuff; }
        public void SetOnlineBuff(string value) { onlineBuff = value; }
        public string GetRecBuff() { return recBuff; }
        public void SetRecBuff(string value) { recBuff = value; }

        public void SetServerInfo(string host, string port)
        {
            this.serverHost = host;
            this.serverPort = port;
        }

        public async Task<string> Connect()
        {
            try
            {
                reconnectAttempts = 0;
                return await ConnectInternal();
            }
            catch (Exception ex)
            {
                Debug.LogError($"连接服务器失败: {ex.Message}");
                return $"连接失败: {ex.Message}";
            }
        }

        private async Task<string> ConnectInternal()
        {
            try
            {
                if (webSocket != null)
                {
                    // await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", CancellationToken.None);
                    webSocket.Dispose();
                }

                webSocket = new ClientWebSocket();
                webSocket.Options.KeepAliveInterval = TimeSpan.FromDays(7);
                string serverUrl = $"ws://{serverHost}:{serverPort}";
                await webSocket.ConnectAsync(new Uri(serverUrl), CancellationToken.None);

                StartReceiving();
                OnConnectionStatusChanged?.Invoke(true);
                return "连接成功";
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged?.Invoke(false);
                Debug.LogError($"连接服务器失败: {ex.Message}");
                return $"连接失败: {ex.Message}";
            }
        }


        private async Task<bool> TryReconnect()
        {
            if (reconnectAttempts >= MaxReconnectAttempts)
            {
                Debug.LogError($"重连失败，已达到最大重试次数 {MaxReconnectAttempts}");
                return false;
            }

            reconnectAttempts++;
            Debug.Log($"尝试重连 ({reconnectAttempts}/{MaxReconnectAttempts})...");

            try
            {
                await Task.Delay(ReconnectDelayMs);
                string result = await ConnectInternal();
                if (result.Contains("成功"))
                {
                    Debug.Log("重连成功");
                    reconnectAttempts = 0;
                    runtimeInstance.isConnected = true;
                    runtimeInstance.StartRealtimeRecognition("2");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"重连失败: {ex.Message}");
            }

            return false;
        }

        private void StartReceiving()
        {
            if (isRunning)
                return;

            isRunning = true;
            ReceiveMessages();
        }

        private async Task ReceiveMessages()
        {
            byte[] buffer = new byte[4096];
            var cancellationToken = cancellationTokenSource.Token;

            while (isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    lock (receiveLock)
                    {
                        if (isReceiving)
                        {
                            Task.Delay(100).Wait();
                            continue;
                        }
                        isReceiving = true;
                    }

                    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                    {
                        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

                        try
                        {
                            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), linkedCts.Token);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                Debug.LogWarning($"服务器请求关闭连接: {result.CloseStatusDescription}");
                                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed connection", cancellationToken);
                                isRunning = false;
                                break;
                            }
                            else if (result.MessageType == WebSocketMessageType.Text)
                            {
                                string message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                                if (runtimeInstance != null)
                                {
                                    runtimeInstance.messageQueue.Enqueue(message);
                                }
                            }
                        }
                        catch (OperationCanceledException e)
                        {
                            Debug.LogWarning($"接收消息超时: {e.Message}");
                            continue;
                        }
                        catch (WebSocketException ex)
                        {
                            Debug.LogError($"WebSocket异常: {ex.Message}");

                            FunasrRuntime.Instance.StopRealtimeRecognition();
                            if (await TryReconnect())
                            {
                                continue;
                            }
                            else
                            {
                                isRunning = false;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"接收消息异常: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    await Task.Delay(1000);
                }
                finally
                {
                    lock (receiveLock)
                    {
                        isReceiving = false;
                    }
                }
            }

            isRunning = false;
            Debug.LogWarning("接收线程已结束");
        }

        // 其他方法的代码不变，但需要在适当的地方使用cancellationTokenSource.Token
        public async void SendAudioData(byte[] audioData)
        {
            if (!IsConnected())
            {
                Debug.LogError("WebSocket未连接");
                return;
            }

            try
            {
                // 计算每块音频的大小 - 根据参数调整
                int chunkSize = waveSampleRate / 1000 * 60 * chunkSizeArray[1] / chunkInterval;

                // 分块发送音频数据
                for (int i = 0; i < audioData.Length; i += chunkSize)
                {
                    // 检查取消标志
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        Debug.Log("发送音频数据被取消");
                        return;
                    }

                    // 每个数据块发送前再次检查连接状态
                    if (!IsConnected())
                    {
                        Debug.LogError("正在发送数据块时WebSocket连接已断开");
                        return;
                    }

                    int length = Math.Min(chunkSize, audioData.Length - i);
                    ArraySegment<byte> segment = new ArraySegment<byte>(audioData, i, length);
                    await webSocket.SendAsync(segment, WebSocketMessageType.Binary, true, cancellationTokenSource.Token);
                    // 发送之间稍等一下，避免连续发送造成网络压力
                    await Task.Delay(1, CancellationToken.None);
                }
            }
            catch (OperationCanceledException)
            {
                // 操作被取消，这是正常的，不需要记录错误
                Debug.Log("发送音频数据被取消");
            }
            catch (Exception ex)
            {
                Debug.LogError($"发送音频数据错误: {ex.Message}");
                isRunning = false;
            }
        }

        public void StartOnlineRecognition(string asrMode)
        {
            if (!IsConnected())
            {
                Debug.LogError("WebSocket未连接，无法开始识别");
                return;
            }

            string firstBuff = string.Format("{{\"mode\": \"{0}\", \"chunk_size\": [{1},{2},{3}], \"chunk_interval\": {4}, \"wav_name\": \"microphone\", \"is_speaking\": true}}",
                asrMode, chunkSizeArray[0], chunkSizeArray[1], chunkSizeArray[2], chunkInterval);

            Debug.Log("开始识别发送初始化消息: " + firstBuff);
            try
            {
                // 使用异步方法发送消息
                _ = SendTextMessage(firstBuff).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Debug.LogError($"发送识别初始化消息失败: {t.Exception?.InnerException?.Message}");
                        // 将详细异常信息发送到主线程
                        if (runtimeInstance != null)
                        {
                            runtimeInstance.messageQueue.Enqueue("ERROR:开始识别失败: " + t.Exception?.InnerException?.Message);
                        }
                    }
                    else
                    {
                        Debug.Log("识别初始化消息发送成功");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"发送识别初始化消息失败: {ex.Message}");
                // 将详细异常信息发送到主线程
                if (runtimeInstance != null)
                {
                    runtimeInstance.messageQueue.Enqueue("ERROR:开始识别失败: " + ex.Message);
                }
            }
        }

        public void EndOnlineRecognition()
        {
            EndOnlineRecognition(true);
        }

        public void EndOnlineRecognition(bool waitForResponse)
        {
            if (isRunning && webSocket != null && webSocket.State == WebSocketState.Open)
            {
                var task = SendTextMessage("{\"is_speaking\": false}");
                if (waitForResponse)
                {
                    try
                    {
                        task.Wait();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"结束识别时发生错误: {ex.Message}");
                    }
                }
            }
        }

        public async void SendAudioFile(string filePath)
        {
            if (!isRunning || webSocket == null || webSocket.State != WebSocketState.Open)
            {
                Debug.LogError("WebSocket未连接");
                return;
            }

            try
            {
                string fileExtension = Path.GetExtension(filePath).ToLower().Replace(".", "");
                if (!(fileExtension == "mp3" || fileExtension == "mp4" || fileExtension == "wav" || fileExtension == "pcm"))
                {
                    Debug.LogError("不支持的文件类型: " + fileExtension);
                    return;
                }

                string firstBuff;
                if (fileExtension == "wav" || fileExtension == "pcm")
                {
                    firstBuff = string.Format("{{\"mode\": \"offline\", \"chunk_size\": [{0},{1},{2}], \"chunk_interval\": {3}, \"wav_name\": \"{4}\", \"is_speaking\": true, \"wav_format\":\"pcm\"}}",
                        chunkSizeArray[0], chunkSizeArray[1], chunkSizeArray[2], chunkInterval, Path.GetFileName(filePath));
                }
                else
                {
                    firstBuff = string.Format("{{\"mode\": \"offline\", \"chunk_size\": [{0},{1},{2}], \"chunk_interval\": {3}, \"wav_name\": \"{4}\", \"is_speaking\": true, \"wav_format\":\"{5}\"}}",
                        chunkSizeArray[0], chunkSizeArray[1], chunkSizeArray[2], chunkInterval, Path.GetFileName(filePath), fileExtension);
                }

                await SendTextMessage(firstBuff);

                // 读取文件并发送 - 使用异步读取文件
                byte[] fileData = await Task.Run(() => File.ReadAllBytes(filePath));

                // 如果是WAV文件，跳过44字节的头
                if (fileExtension == "wav")
                {
                    fileData = fileData.Skip(44).ToArray();
                }

                const int fileChunkSize = 102400;
                int totalChunks = (fileData.Length + fileChunkSize - 1) / fileChunkSize; // 向上取整

                // 添加进度报告
                double lastProgressReported = 0;

                for (int i = 0; i < fileData.Length; i += fileChunkSize)
                {
                    int length = Math.Min(fileChunkSize, fileData.Length - i);
                    ArraySegment<byte> segment = new ArraySegment<byte>(fileData, i, length);
                    await webSocket.SendAsync(segment, WebSocketMessageType.Binary, true, CancellationToken.None);

                    // 每发送几个块暂停一下，避免一次性发送太多数据
                    if (i % (fileChunkSize * 10) == 0)
                    {
                        await Task.Delay(10);
                    }

                    // 报告进度（每10%报告一次）
                    double progress = (double)i / fileData.Length;
                    if (progress - lastProgressReported >= 0.1)
                    {
                        lastProgressReported = progress;
                        Debug.Log($"文件发送进度: {progress:P0}");
                    }
                }

                await Task.Delay(100);
                await SendTextMessage("{\"is_speaking\": false}");
                Debug.Log("文件发送完成: " + filePath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"发送文件错误: {ex.Message}");
            }
        }

        // 将ParseJson方法设为public，以便FunasrRuntime可以访问
        public Dictionary<string, object> ParseJson(string jsonString)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            // 非常简单的JSON解析，仅适用于已知的FunASR响应格式
            // 实际应用中应使用完整的JSON解析库
            try
            {
                jsonString = jsonString.Trim();
                if (jsonString.StartsWith("{") && jsonString.EndsWith("}"))
                {
                    // 移除花括号
                    jsonString = jsonString.Substring(1, jsonString.Length - 2);

                    // 分割属性
                    string[] pairs = jsonString.Split(',');
                    foreach (string pair in pairs)
                    {
                        string[] keyValue = pair.Split(':');
                        if (keyValue.Length == 2)
                        {
                            string key = keyValue[0].Trim().Trim('"');
                            string value = keyValue[1].Trim();

                            // 处理不同类型的值
                            if (value.StartsWith("\"") && value.EndsWith("\""))
                            {
                                // 字符串
                                result[key] = value.Trim('"');
                            }
                            else if (value.ToLower() == "true" || value.ToLower() == "false")
                            {
                                // 布尔值
                                result[key] = bool.Parse(value);
                            }
                            else if (value.Contains("."))
                            {
                                // 尝试解析为小数
                                double doubleValue;
                                if (double.TryParse(value, out doubleValue))
                                    result[key] = doubleValue;
                                else
                                    result[key] = value;
                            }
                            else
                            {
                                // 尝试解析为整数
                                int intValue;
                                if (int.TryParse(value, out intValue))
                                    result[key] = intValue;
                                else
                                    result[key] = value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("解析JSON出错: " + ex.Message);
            }

            return result;
        }

        // 添加一个方法用于检查连接状态
        public bool IsConnected()
        {
            return isRunning && webSocket != null && webSocket.State == WebSocketState.Open;
        }

        // SendTextMessage方法需要支持取消
        private async Task SendTextMessage(string message)
        {
            if (webSocket == null || webSocket.State != WebSocketState.Open)
                return;

            try
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // 操作被取消，这是正常的，不需要记录错误
                Debug.Log("发送消息被取消");
            }
            catch (Exception ex)
            {
                Debug.LogError($"发送消息错误: {ex.Message}");
            }
        }

        public void CloseConnection()
        {
            // 首先取消所有正在进行的操作
            try
            {
                if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"取消WebSocket操作出错: {ex.Message}");
            }

            // 标记为不再运行
            isRunning = false;

            if (webSocket != null)
            {
                try
                {
                    // 创建一个新的取消令牌，设置超时时间为3秒
                    using (var closeCts = new CancellationTokenSource(3000))
                    {
                        // 使用超时机制关闭连接，避免长时间阻塞
                        if (webSocket.State == WebSocketState.Open)
                        {
                            var closeTask = webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", closeCts.Token);

                            // 不等待关闭完成，避免阻塞主线程
                            closeTask.ContinueWith(t =>
                            {
                                if (t.IsFaulted)
                                {
                                    Debug.LogError($"关闭WebSocket出错: {t.Exception?.InnerException?.Message}");
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"关闭WebSocket连接错误: {ex.Message}");
                }
                finally
                {
                    try
                    {
                        // 确保在finally中释放资源
                        webSocket.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"释放WebSocket资源出错: {ex.Message}");
                    }

                    webSocket = null;
                }
            }

            // 确保所有任务都被正确处理
            try
            {
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
            }
            catch (Exception ex)
            {
                Debug.LogError($"释放取消令牌资源出错: {ex.Message}");
            }

            Debug.Log("WebSocket连接已关闭");
        }
    }
    #endregion

    // 麦克风设备列表滚动位置
    private Vector2 scrollPosition = Vector2.zero;
}
