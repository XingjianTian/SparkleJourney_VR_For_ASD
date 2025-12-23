using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

class AudioClipEvent
{
    public AudioClip clip;
    public UnityAction callback;

    public AudioClipEvent(AudioClip clip, UnityAction callback = null)
    {
        this.clip = clip;
        this.callback = callback;
    }
}

public class TextEvent
{
    public string content;
    public UnityAction callback;

    public TextEvent(string content, UnityAction callback = null)
    {
        this.content = content;
        this.callback = callback;
    }
}


public class DeepseekAsistant : MonoSingleton<DeepseekAsistant>
{


    public TextMeshProUGUI outputText;
    protected override void Awake()
    {
        base.Awake();
        AppleForMicorphonePower();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.volume = 0.8f;
        _systemAudioSource = gameObject.AddComponent<AudioSource>();
        _systemAudioSource.volume = 1f;
        if (outputText != null)
        {
            _typewriterByCharacter = outputText.GetComponent<TypewriterByCharacter>();
            _typewriterByCharacter.onTextDisappeared.AddListener((() =>
            {
                outputText.transform.parent.gameObject.SetActive(false);
            }));
        }
    }
    /// <summary>
    /// 请求麦克风权限
    /// </summary>
    private void AppleForMicorphonePower()
    {
        if (!(Microphone.devices.Length > 0))
        {
            Debug.LogWarning("没有麦克分权限，准备申请");
            Microphone.Start(null, false, 10, 44100);
            Microphone.End(null);
        }
    }
    private AudioSource _audioSource;
    private AudioSource _systemAudioSource;
    private AudioClip clip;

    public Queue<TextEvent> textQueue = new Queue<TextEvent>();

    private Queue<AudioClipEvent> clipQueue = new Queue<AudioClipEvent>();
    private Queue<AudioClipEvent> systemClipQueue = new Queue<AudioClipEvent>();


    private TypewriterByCharacter _typewriterByCharacter;
    // Start is called before the first frame update

    public InputActionReference skipDialogBtn;





    public void TalkWithAi(string problem)
    {
        SiliconFlowDeepSeekHandle.Instance.Send(problem, textQueue);
    }

    // Update is called once per frame
    void Update()
    {
        HandleText2Speech();
        HandleAudioSpeech();
        HandleSystemAudioSpeech();


    }





    public bool IsInteractionSpeeching
    {
        get
        {
            if (_audioSource.isPlaying || textQueue.Count > 0 || isProcessing || clipQueue.Count > 0)
            {
                return true;
            }

            return false;
        }
    }

    public void AbortInteractionSpeech()
    {
        if (outputText != null)
        {
            outputText.text = "";
        }
        isProcessing = false;
        //防止api返回的内容持续添加到队列中
        textQueue = new Queue<TextEvent>();
        clipQueue.Clear();
        _audioSource.Stop();
        _audioSource.clip = null;
    }

    private bool isProcessing;
    private string preDisappearId;
    private async Task HandleText2Speech()
    {
        if (textQueue.Count > 0 && !isProcessing)
        {
            isProcessing = true;
            TextEvent line = textQueue.Dequeue();
            if (outputText != null)
            {
                if (line.content.Contains("测试"))
                {
                    return;
                }
                outputText.transform.parent.gameObject.SetActive(true);
                outputText.text += line.content.Replace(";;","");
                if (preDisappearId != null)
                {
                    TimerMgr.Instance.StopTimer(preDisappearId);
                }

                preDisappearId = TimerMgr.Instance.StartTimer(3f, (() =>
                {
                    _typewriterByCharacter.StartDisappearingText();

                }));

            }
            await Text2SpeechHandle.Instance.Send(line.content, (clip =>
            {
                if (isProcessing)
                {
                    isProcessing = false;
                    AudioClipEvent audioClipEvent = new AudioClipEvent(clip, line.callback);

                    clipQueue.Enqueue(audioClipEvent);
                }

            }),false);
        }
    }

    private AudioClipEvent prevCommonAudioClipEvent;
    private void HandleAudioSpeech()
    {
        if (!_audioSource.isPlaying)
        {
            prevCommonAudioClipEvent?.callback?.Invoke();
            prevCommonAudioClipEvent = null;
        }
        if (clipQueue.Count > 0 && !_audioSource.isPlaying)
        {

            AudioClipEvent audioClipEvent = clipQueue.Dequeue();
            _audioSource.clip = audioClipEvent.clip;
            _audioSource.Play();
            prevCommonAudioClipEvent = audioClipEvent;
        }

    }

    private AudioClipEvent prevSystemAudioClipEvent;
    private void HandleSystemAudioSpeech()
    {
        if (!_systemAudioSource.isPlaying)
        {
            prevSystemAudioClipEvent?.callback?.Invoke();
            prevSystemAudioClipEvent = null;
        }

        if (systemClipQueue.Count > 0 && !_systemAudioSource.isPlaying)
        {
            AudioClipEvent audioClipEvent = systemClipQueue.Dequeue();
            _systemAudioSource.clip = audioClipEvent.clip;
            _systemAudioSource.Play();
            prevSystemAudioClipEvent = audioClipEvent;
        }

    }
    public void SpeechByString(string content, UnityAction callback = null)
    {
        AbortInteractionSpeech();
        textQueue.Enqueue(new TextEvent(content, callback));
    }

    public void SpeechBuildIn(string path, UnityAction callback = null)
    {
        AbortInteractionSpeech();
        systemClipQueue.Enqueue(new AudioClipEvent(Resources.Load<AudioClip>(path), callback));
    }
    public void SpeechBuildIn(AudioClip clip, UnityAction callback = null)
    {
        AbortInteractionSpeech();
        systemClipQueue.Enqueue(new AudioClipEvent(clip, callback));
    }


}
