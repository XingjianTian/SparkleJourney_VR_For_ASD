using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System;


public class AutoFunasrRecognitionForRobot : MonoBehaviour
{


    public FunasrRuntime funasrRuntime;

    public float activeContinueTime = 2f;
    private async void Start()
    {
        if (funasrRuntime == null)
        {
            funasrRuntime = gameObject.AddComponent<FunasrRuntime>();
        }

        await Task.Delay(1);
        string res = await funasrRuntime.ConnectToServer();
        print(res);
        funasrRuntime.SetResultCallback(OnResultCallback);
        funasrRuntime.SetInterimResultCallback(OnTempResultCallback);

        funasrRuntime.StartRealtimeRecognition("2");

        StartCoroutine(HandleRecognitionMessage());
    }




    IEnumerator HandleRecognitionMessage()
    {
        while (true)
        {
            if (isActive)
                passedTime += Time.deltaTime;
            if (isActive && passedTime > activeContinueTime)
            {
                isActive = false;
                passedTime = 0;
                Debug.LogError(recognitedMessage);
                if (recognitedMessage.Length > 0)
                {
                    DeepseekAsistant.Instance.TalkWithAi(recognitedMessage);
                }
            }
            yield return null;
        }
    }


    private bool isActive;

    public float passedTime;
    public string recognitedMessage;
    private void OnResultCallback(string content)
    {
        if ((content.IndexOf("小心") != -1 || content.IndexOf("小星") != -1) && !isActive)
        {

            RobotObj.Instance.SpeechInResorucesAudio("Music/EnvMusic/YY Room/awake",(() =>
            {
                funasrRuntime.ClearRecBuff();
                isActive = true;
                passedTime = 0;
                recognitedMessage = "";
            }));
        }

        if (isActive)
        {
            recognitedMessage = content;
            passedTime = 0;
        }
    }

    private void OnTempResultCallback(string content)
    {
        if (isActive)
        {
            recognitedMessage = content;
            passedTime = 0;
        }
    }



}