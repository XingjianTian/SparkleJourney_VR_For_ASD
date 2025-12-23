using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicSceneMgr : MonoBehaviour
{
    private void Start()
    {
        RobotObj.Instance.animator.SetTrigger("Hellow");
        RobotObj.Instance.SpeechText("哈喽，小朋友，欢迎来到音乐空间，在这里享受音乐的乐趣吧",(() =>
        {
            TimerMgr.Instance.StartTimer(0.5f, (() =>
            {
                RobotObj.Instance.SpeechText("现在你可以试试弹弹钢琴，尝试一下非洲鼓");

            }));
        }));
        TimerMgr.Instance.StartTimer(3, (() =>
        {
            RobotObj.Instance.SpeechText("当你不想玩了可以靠近出生点的门，他会带你回到房间哦");
        }));
    }
}
