# FunASR Unity客户端

本项目是基于FunASR语音识别服务的Unity客户端，支持实时语音识别和音频文件转写功能。

## 功能特点

- 实时语音识别（在线模式和动态修正模式）
- 音频文件转写（支持WAV、MP3、MP4、PCM格式）
- 麦克风设备选择功能
- 事件系统，方便集成到各种应用中
- 简单易用的UI界面示例

## 使用方法

### 1. 基础设置

1. 将`Funasr`文件夹导入到Unity项目中
2. 在场景中创建一个空对象，添加`FunasrRuntime`组件
3. 设置服务器地址和端口

### 2. 示例UI

项目包含一个示例UI，提供了连接服务器、开始/停止识别、文件转写等功能：

1. 在场景中创建一个Canvas
2. 添加`FunasrSample`组件到Canvas或其子对象上
3. 设置UI引用（按钮、文本框等）

### 3. 直接通过代码使用

```csharp
// 获取FunasrRuntime实例
FunasrRuntime funasrRuntime = GetComponent<FunasrRuntime>();

// 设置服务器信息
funasrRuntime.host = "127.0.0.1";
funasrRuntime.port = "10096";

// 连接到服务器
await funasrRuntime.ConnectToServer();

// 设置回调
funasrRuntime.SetResultCallback(result => {
    Debug.Log("识别结果: " + result);
});

// 开始实时识别 ("1"=在线模式, "2"=动态修正模式)
funasrRuntime.StartRealtimeRecognition("2");

// 停止识别
funasrRuntime.StopRealtimeRecognition();

// 转写音频文件
funasrRuntime.TranscribeAudioFile("/path/to/audio.wav");
```

### 4. 事件监听

您可以通过以下两种方式设置事件监听：

#### 通过FunasrRuntime方法（推荐）

```csharp
funasrRuntime.SetResultCallback(OnRecognitionResult);
funasrRuntime.SetInterimResultCallback(OnInterimResult);
funasrRuntime.SetConnectionStatusCallback(OnConnectionStatus);
```

#### 通过全局事件

```csharp
FunasrEvents.OnRecognitionResult += OnRecognitionResult;
FunasrEvents.OnInterimResult += OnInterimResult;
FunasrEvents.OnConnectionStatus += OnConnectionStatus;
FunasrEvents.OnServerError += OnServerError;
FunasrEvents.OnRecognitionEnd += OnRecognitionEnd;
```

### 5. 麦克风设备选择

FunASR客户端支持选择不同的麦克风设备进行录音。

#### 使用内置的设备选择界面

```csharp
// 显示麦克风设备选择界面
funasrRuntime.ShowMicrophoneSelectionGUI(true);

// 隐藏麦克风设备选择界面
funasrRuntime.ShowMicrophoneSelectionGUI(false);
```

#### 通过代码设置麦克风设备

```csharp
// 获取所有可用的麦克风设备
string[] devices = funasrRuntime.GetMicrophoneDevices();

// 获取当前选择的麦克风设备
string currentDevice = funasrRuntime.GetSelectedMicrophoneDevice();

// 设置要使用的麦克风设备
funasrRuntime.SetMicrophoneDevice("设备名称"); // 使用特定设备
funasrRuntime.SetMicrophoneDevice(null); // 使用默认设备
```

## 注意事项

1. 确保FunASR服务已运行且能够访问
2. 识别前确保已成功连接到服务器
3. 使用完后请调用`StopRealtimeRecognition()`方法停止识别
4. 如果切换麦克风设备，请先停止当前识别 