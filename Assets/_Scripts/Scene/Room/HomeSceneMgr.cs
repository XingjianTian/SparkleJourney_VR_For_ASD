using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HomeSceneMgr : MonoBehaviour
{
    public static string currentMode = "First";


    public BoxCollider b1;
    public BoxCollider b2;

    public Transform musicDoorPos;

    public Transform freeDrawDoorPos;

    [Header("input")] public InputActionReference rightAKey;
    // Start is called before the first frame update
    void Start()
    {

        if (currentMode == "First")
        {
            RobotObj.Instance.animator.SetTrigger("Hellow");
            RobotObj.Instance.SpeechText("欢迎来到你的房间，在这里可以尽情玩耍哦",(() =>
            {
                TimerMgr.Instance.StartTimer(100f,(() =>
                {
                    RobotObj.Instance.SpeechText("相信你在房间里肯定玩的很开心，接下来让我们去音乐空间吧",(() =>
                    {
                        NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, musicDoorPos.position);
                        b1.enabled = true;
                        currentMode = "Second";
                    }));
                }));

            }));
        } else if (currentMode == "Second")
        {
            RobotObj.Instance.SpeechText("下面让我们体验一下空间绘画吧，跟我来",(() =>
            {
                NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, freeDrawDoorPos.position);
                b2.enabled = true;
                currentMode = "Third";
            }));
        } else if (currentMode == "Third")
        {
            RobotObj.Instance.SpeechText("欢迎回来，恭喜你完成了自由空间的任务",(() =>
            {
                TimerMgr.Instance.StartTimer(0.5f, () =>
                {
                    RobotObj.Instance.SpeechText("按下右手柄A键可以继续体验其他空间哦");
                    rightAKey.action.performed += HandleBack2Main;
                });

            }));
        }

    }

    private void HandleBack2Main(InputAction.CallbackContext callbackContext)
    {
        currentMode = "Done";
        rightAKey.action.performed -= HandleBack2Main;
        SceneManager.LoadSceneAsync("0_BeginScene");
    }
    // Update is called once per frame
    void Update()
    {

        // 计算需要面向的方向（保持Y轴不变，只在水平面上旋转）
        Vector3 targetPosition = PlayerObj.Instance.transform.position;
        Vector3 robotPosition = RobotObj.Instance.transform.position;
        targetPosition.y = robotPosition.y; // 保持y轴高度相同

        // 计算朝向
        Vector3 direction = targetPosition - robotPosition;
        if (direction != Vector3.zero) // 防止零向量
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            RobotObj.Instance.transform.rotation = targetRotation;

        }
    }
}
