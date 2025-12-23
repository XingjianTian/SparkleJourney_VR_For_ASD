using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneHandle : BaseMgr<MicrophoneHandle>
{
    private MicrophoneHandle()
    {
        foreach (var item in Microphone.devices)
        {
            Debug.Log(item);
        }

        if (Microphone.devices.Length > 0)
        {
            isEnabled = true;

#if UNITY_EDITOR
            defaultDeviceName = Microphone.devices[2];
#else
            defaultDeviceName = Microphone.devices[0];
#endif
        }
        else
        {
            Debug.LogWarning("没有找到麦克风");
        }
    }

    private bool isEnabled;
    private int recordFrequency  = 16000;
    private int maxRecoardTime = 60;

    private string defaultDeviceName;
    private AudioClip currentRecordedClip;
    public bool StartRecord()
    {
        if (!isEnabled || Microphone.IsRecording(defaultDeviceName))
        {
            Debug.LogWarning($"{defaultDeviceName} 无法开始录音");
            return false;
        }

        currentRecordedClip =Microphone.Start(defaultDeviceName, false, maxRecoardTime, recordFrequency);
        return true;
    }

    public AudioClip StopRecord()
    {
        if (!isEnabled || !Microphone.IsRecording(defaultDeviceName))
        {
            Debug.LogWarning($"{defaultDeviceName}无法结束录音");
            return null;
        }

        int clipPos = Microphone.GetPosition(defaultDeviceName);
        Microphone.End(defaultDeviceName);
        return TrimAudioClip(currentRecordedClip, clipPos);
    }

    private AudioClip TrimAudioClip(AudioClip clip, int recordedLength)
    {
        float[] sample = new float[clip.samples * clip.channels];
        clip.GetData(sample, 0);

        float[] trimedSample = new float[recordedLength * clip.channels];
        Array.Copy(sample,trimedSample,recordedLength * clip.channels);
        AudioClip trimedClip = AudioClip.Create("TrimmedClip", recordedLength, clip.channels, clip.frequency, false);
        trimedClip.SetData(trimedSample,0);
        return trimedClip;

    }
}