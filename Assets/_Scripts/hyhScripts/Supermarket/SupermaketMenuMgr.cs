using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum SupermaketMenuType
{
    OpenMenu,
    CloseMenu,
}
public class SupermaketMenuMgr : MonoBehaviour
{
    public static bool isDone;
    public InputActionReference rightAkey;
    public InputActionReference rightBkey;
    public SupermaketMenuType menuType;
    public Transform target;
    private void HandleStartBall(InputAction.CallbackContext context)
    {
        BallController.Instance.ThrowBall();

        rightAkey.action.performed -= HandleStartBall;
    }


    private void HandleBack2Main(InputAction.CallbackContext context)
    {
        SceneManager.LoadSceneAsync("0_BeginScene");
        rightBkey.action.performed -= HandleBack2Main;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isDone)
            {
                ShoppingPanel.Instance.Hide();
                TimerMgr.Instance.StartTimer(0.5f, (() =>
                {
                    ShoppingPanel.Instance.Hide();
                }));
                //传送
                Transform setupTrans = PlayerObj.Instance.transform.parent.parent;
                setupTrans.position = target.position;
                setupTrans.rotation = target.rotation;
                RobotObj.Instance.SpeechText("现在让我们来打会球运动一下吧，按下右手柄的A键开始游戏哦",(() =>
                {
                    rightAkey.action.performed += HandleStartBall;
                    TimerMgr.Instance.StartTimer(10, (() =>
                    {
                        RobotObj.Instance.SpeechText("如果你玩累了，可以按下右手柄的B键可以返回主场景哦");
                        rightBkey.action.performed += HandleBack2Main;
                    }));
                }));
            }
            switch (menuType)
            {
                case SupermaketMenuType.OpenMenu:
                    ShoppingPanel.Instance.Show();
                    NavUtility.Instance.HideAllNavPath();
                    break;
                case SupermaketMenuType.CloseMenu:
                    ShoppingPanel.Instance.Hide();
                    break;
            }
        }
    }
}
