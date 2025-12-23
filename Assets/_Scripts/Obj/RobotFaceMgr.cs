using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class RobotFaceMgr : AutoMonoSingleton<RobotFaceMgr>
{
    [Header("组件引用")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("表情视频配置")]
    [SerializeField] private VideoClip defaultEmotion;
    [SerializeField] private List<EmotionData> emotions = new List<EmotionData>();

    private Dictionary<string, VideoClip> emotionDictionary = new Dictionary<string, VideoClip>();
    private bool isInitialized = false;

    [System.Serializable]
    public class EmotionData
    {
        public string emotionName;
        public VideoClip videoClip;
    }

    void Start()
    {
        InitializeVideoPlayer();
    }

    private void InitializeVideoPlayer()
    {
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                videoPlayer = gameObject.AddComponent<VideoPlayer>();
            }
        }




        // 初始化表情字典
        foreach (EmotionData emotion in emotions)
        {
            if (emotion.videoClip != null && !string.IsNullOrEmpty(emotion.emotionName))
            {
                emotionDictionary[emotion.emotionName] = emotion.videoClip;
            }
        }

        isInitialized = true;

        // 如果存在默认表情，播放它
        if (defaultEmotion != null)
        {
            ShowEmotion(defaultEmotion);
        }
    }

    /// <summary>
    /// 显示指定的表情
    /// </summary>
    /// <param name="emotionName">表情名称，必须与配置中的名称匹配</param>
    /// <param name="onCompleted">表情播放完成后的回调</param>
    /// <param name="loop">是否循环播放</param>
    /// <returns>是否成功播放表情</returns>
    public bool ShowEmotion(string emotionName, UnityEngine.Events.UnityAction onCompleted = null, bool loop = false)
    {
        if (!isInitialized)
        {
            InitializeVideoPlayer();
        }

        if (emotionDictionary.TryGetValue(emotionName, out VideoClip clip))
        {
            return ShowEmotion(clip, onCompleted, loop);
        }
        else
        {
            Debug.LogWarning($"找不到表情: {emotionName}");
            return false;
        }
    }

    /// <summary>
    /// 直接显示指定的视频片段作为表情
    /// </summary>
    /// <param name="clip">视频片段</param>
    /// <param name="onCompleted">表情播放完成后的回调</param>
    /// <param name="loop">是否循环播放</param>
    /// <returns>是否成功播放表情</returns>
    private bool ShowEmotion(VideoClip clip, UnityEngine.Events.UnityAction onCompleted = null, bool loop = true)
    {
        if (!isInitialized)
        {
            InitializeVideoPlayer();
        }

        if (clip == null)
        {
            Debug.LogWarning("无法播放空的表情视频");
            return false;
        }

        // 停止当前正在播放的视频
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        // 设置新的视频剪辑
        videoPlayer.clip = clip;
        videoPlayer.isLooping = loop;

        // 设置完成回调，确保只执行一次
        if (onCompleted != null)
        {
            // 创建一个一次性事件处理程序
            UnityEngine.Video.VideoPlayer.EventHandler handler = null;
            handler = (vp) =>
            {
                // 执行回调
                onCompleted.Invoke();

                // 移除事件处理程序，确保回调只执行一次
                videoPlayer.loopPointReached -= handler;
            };

            // 添加事件处理程序
            videoPlayer.loopPointReached += handler;
        }

        videoPlayer.Play();

        return true;
    }


    void Update()
    {
        // if (Keyboard.current.oKey.wasPressedThisFrame)
        // {
        //     print(1);
        //     ShowEmotion("huanhu");
        // }
        // if (Keyboard.current.mKey.wasPressedThisFrame)
        // {
        //     print(2);
        //     ShowEmotion("jiayou");
        // }

    }
}
