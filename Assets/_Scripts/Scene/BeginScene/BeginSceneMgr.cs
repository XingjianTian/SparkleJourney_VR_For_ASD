using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;







public class BeginSceneMgr : MonoSingleton<BeginSceneMgr>
{
    public static string currentMode = "First";

    public Transform teachArea;
    public GameObject joystickCanvas;
    public InputActionReference leftJoystick;

    public InputActionReference rightAkey;

    private Coroutine nav2TeachAreaPath = null;
    [Header("Door Pos")] public Transform freezenDoorPos;
    public Transform classDoorPos;
    public Transform interflowDoorPos;
    public Transform multipleDoorPos;
    public void TriggerEnterTeachAreaPoint(UnityAction callback)
    {
        if (nav2TeachAreaPath != null)
        {
            NavUtility.Instance.HideNavPath(nav2TeachAreaPath);
            RobotObj.Instance.SpeechText("很好，恭喜你完成了第一个小目标",(() =>
            {
                callback?.Invoke();
            }));
        }
        else
        {
            callback?.Invoke();
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        if (currentMode == "First")
        {
            RobotObj.Instance.animator.SetTrigger("Hellow");
            RobotObj.Instance.SpeechText("小朋友，欢迎来到星火童行，接下来让我带你探索这神秘的世界吧",(() =>
            {
                TimerMgr.Instance.StartTimer(0.5f, (() =>
                {
                    RobotObj.Instance.SpeechText("如果不熟悉手柄操作的话，可以通过左手柄的遥感控制移动方向进入教程区域哦",(() =>
                    {
                        RobotObj.Instance.SpeechText("如果你已经很熟悉操作了，可以按下右手柄的A键结束新手教程");
                        rightAkey.action.performed += HandleEndPrimaryTeach;
                    }));
                    joystickCanvas.SetActive(true);
                    nav2TeachAreaPath = NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, teachArea.position);
                    leftJoystick.action.performed += HandleStartPrimaryTeach;

                }));

            }));
        } else if (currentMode == "Second")
        {
            RobotObj.Instance.SpeechText("很好，恭喜你完成了自由空间的任务，接下来让我们挑战更高难度的传送门吧",(() =>
            {
                NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, classDoorPos.position);
                RobotObj.Instance.SpeechText("让我们跟着地上的指引进入游戏空间吧，放心，我将时刻伴你左右");
            }));
        } else if (currentMode == "Third")
        {
            RobotObj.Instance.SpeechText("恭喜你完成游戏空间，下面让我们挑战一下社交空间吧，相信你肯定很感兴趣",(() =>
            {
                NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, interflowDoorPos.position);
                RobotObj.Instance.SpeechText("让我们跟着地上的指引进入社交空间");
            }));
        } else if (currentMode == "Forth")
        {
            RobotObj.Instance.SpeechText("很不错，让我们进入多人联机空间吧",(() =>
            {
                NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, multipleDoorPos.position);
                // RobotObj.Instance.SpeechText("跟着地上的指引进入多人联机空间，这里可以和小伙伴一起完成任务哦");
            }));
        }
    }

    private void HandleStartPrimaryTeach(InputAction.CallbackContext context)
    {
        rightAkey.action.performed -= HandleEndPrimaryTeach;
        RobotObj.Instance.SpeechText("很好，你已经学会了使用左手柄遥感控制移动啦，现在继续移动前往教程区域");
        TimerMgr.Instance.StartTimer(0.5f, () =>
        {
            joystickCanvas.SetActive(false);
        });
        leftJoystick.action.performed -= HandleStartPrimaryTeach;
    }


    public void HandleEndPrimaryTeach(InputAction.CallbackContext context)
    {
        rightAkey.action.performed -= HandleEndPrimaryTeach;
        leftJoystick.action.performed -= HandleStartPrimaryTeach;
        NavUtility.Instance.HideNavPath(nav2TeachAreaPath);
        RobotObj.Instance.SpeechText("接下来让我们进入自由空间吧",(() =>
        {
            NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, freezenDoorPos.position);
        }));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
