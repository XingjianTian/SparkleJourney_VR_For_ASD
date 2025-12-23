using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

public class TeachArea : MonoSingleton<TeachArea>
{

    public GameObject teachScreen;

    public GameObject group1;
    public GameObject group2;

    public GameObject grabInteractibleObj;

    public InputActionReference LeftGrab;
    public InputActionReference RightGrab;
    public InputActionReference RightStickTeleport;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        BeginSceneMgr.Instance.TriggerEnterTeachAreaPoint((() =>
        {
            teachScreen.SetActive(true);
            RobotObj.Instance.SpeechText("让我们开始学习手柄的操作吧",(() =>
            {
                TimerMgr.Instance.StartTimer(0.5f, (() =>
                {
                    RobotObj.Instance.SpeechText("首先我们先来认识抓取键，通过保持长按可以抓取可交互的物品",(() =>
                    {
                        group1.SetActive(true);
                        TimerMgr.Instance.StartTimer(0.5f, (() =>
                        {
                            RobotObj.Instance.SpeechText("现在让我们尝试一下抓取物品吧",(() =>
                            {
                                grabInteractibleObj.SetActive(true);
                                LeftGrab.action.performed += GrabEventAttack;
                                RightGrab.action.performed += GrabEventAttack;
                            }));

                        }));
                    }));
                }));
            }));
        }));

    }

    private void GrabEventAttack(InputAction.CallbackContext context)
    {
        LeftGrab.action.performed -= GrabEventAttack;
        RightGrab.action.performed -= GrabEventAttack;
        group1.SetActive(false);
        RobotObj.Instance.SpeechText("完成得很不错，继续加油",(() =>
        {
            TimerMgr.Instance.StartTimer(0.5f, () =>
            {
                RobotObj.Instance.SpeechText("接下来让我来学习传送吧，右手柄的摇杆向前推，出现传送指示线后，松开遥感即可传送到目标位置",(() =>
                {
                    group2.SetActive(true);
                    RobotObj.Instance.SpeechText("下面让我们试试看吧",(() =>
                    {
                        RightStickTeleport.action.performed += TeleportEventAttack;
                    }));
                }));
            });
        }));


    }


    private void TeleportEventAttack(InputAction.CallbackContext context)
    {
        group2.SetActive(false);
        RightStickTeleport.action.performed -= TeleportEventAttack;
        RobotObj.Instance.SpeechText("完成的很好，恭喜你完成了基础操作的学习",(() =>
        {
            teachScreen.SetActive(false);
            if (HomeSceneMgr.currentMode == "First")
            {
                RobotObj.Instance.SpeechText("接下来让我们进入自由空间吧",(() =>
                {
                    NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, BeginSceneMgr.Instance.freezenDoorPos.position);
                }));
            }
        }));
    }

    private void OnTriggerExit(Collider other)
    {
        teachScreen.SetActive(false);
        grabInteractibleObj.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
