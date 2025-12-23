using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ClassRoomMgr : MonoSingleton<ClassRoomMgr>
{

    // Start is called before the first frame update
    void Start()
    {
        RobotObj.Instance.animator.SetTrigger("Hellow");
        RobotObj.Instance.SpeechText("欢迎来到游戏空间，在这里享受游戏的乐趣吧", () =>
        {
            TimerMgr.Instance.StartTimer(0.5f, (() =>
            {
                RobotObj.Instance.SpeechText("现在你可以坐上桌子旁的椅子开始游戏哦", () =>
                {
                    RobotObj.Instance.SpeechText("如果不想玩了，可以靠经你身后的门回到开始场景哦");
                });
            }));
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
