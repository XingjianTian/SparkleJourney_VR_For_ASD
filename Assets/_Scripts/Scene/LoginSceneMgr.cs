using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public enum LoginSceneMode
{
    First,
    Second,
    Third,
    Fourth
}


public class LoginSceneMgr : MonoSingleton<LoginSceneMgr>
{
    // public static Dictionary<int, DoorObj> TeleportColliderDoorsDictionary = new Dictionary<int, DoorObj>();
    //
    // public static void RegistryTeleport(int id, DoorObj door)
    // {
    //     TeleportColliderDoorsDictionary.Add(id, door);
    // }



    [SerializeField]
    private List<Transform> robotWayPointsList = new List<Transform>();
    private int currentIndex;

    public static LoginSceneMode currentMode = LoginSceneMode.First;

    [Header("first")] public GameObject GuidePanel;



    private Transform GetPoint(int index)
    {
        if (index < robotWayPointsList.Count)
        {
            return robotWayPointsList[index];
        }
        Debug.LogError($"当前场景没有第{index}个路径点了");
        return null;
    }
    // Start is called before the first frame update
    void Start()
    {
        NavUtility.Instance.HideAllNavPath();
        switch (currentMode)
        {
            case LoginSceneMode.First:
                RobotObj.Instance.SetDestination(GetPoint(0).position,(() =>
                {
                    Vector3 playerPos = PlayerObj.Instance.transform.position;
                    playerPos.y = RobotObj.Instance.transform.position.y;
                    RobotObj.Instance.transform.DORotateQuaternion(
                        Quaternion.LookRotation(playerPos -
                                                RobotObj.Instance.transform.position), 1f);
                    RobotObj.Instance.animator.SetTrigger("Hellow");

                    RobotObj.Instance.SpeechText("你好！我是小星，欢迎来到星火同行",(() =>
                    {
                        GuidePanel.SetActive(true);

                    }));
                }));
                break;
            case LoginSceneMode.Second:
                //进入教室场景提示词
                RobotObj.Instance.SpeechText("接下来，让我带你去游戏空间，跟我来",(() =>
                {
                    NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, GetPoint(3).position);
                    RobotObj.Instance.SetDestination(GetPoint(4).position,(() =>
                    {
                        Vector3 playerPos = PlayerObj.Instance.transform.position;
                        playerPos.y = RobotObj.Instance.transform.position.y;
                        RobotObj.Instance.transform.DORotateQuaternion(
                            Quaternion.LookRotation(playerPos -
                                                    RobotObj.Instance.transform.position), 1f);
                    }));
                }));
                break;
            case LoginSceneMode.Third:
                RobotObj.Instance.SpeechText("接下来，让我带你去社交空间，跟我来",(() =>
                {
                    NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, GetPoint(5).position);
                    RobotObj.Instance.SetDestination(GetPoint(6).position,(() =>
                    {
                        Vector3 playerPos = PlayerObj.Instance.transform.position;
                        playerPos.y = RobotObj.Instance.transform.position.y;
                        RobotObj.Instance.transform.DORotateQuaternion(
                            Quaternion.LookRotation(playerPos -
                                                    RobotObj.Instance.transform.position), 1f);
                    }));
                }));
                break;
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    // private float door1DissolveNum = 1;
    // private float Door1DissolveProcess
    // {
    //     get
    //     {
    //         return door1DissolveNum;
    //     }
    //     set
    //     {
    //         door1DissolveNum = value;
    //         TeleportColliderDoorsDictionary[0].SetDissolveNum(door1DissolveNum);
    //     }
    // }
    // private float door2DissolveNum = 1;
    // private float Door2DissolveProcess
    // {
    //     get
    //     {
    //         return door2DissolveNum;
    //     }
    //     set
    //     {
    //         door2DissolveNum = value;
    //         TeleportColliderDoorsDictionary[1].SetDissolveNum(door2DissolveNum);
    //     }
    // }
    // private float door3DissolveNum = 1;
    // private float Door3DissolveProcess
    // {
    //     get
    //     {
    //         return door3DissolveNum;
    //     }
    //     set
    //     {
    //         door3DissolveNum = value;
    //         TeleportColliderDoorsDictionary[2].SetDissolveNum(door3DissolveNum);
    //     }
    // }
    // private float door4DissolveNum = 1;
    // private float Door4DissolveProcess
    // {
    //     get
    //     {
    //         return door4DissolveNum;
    //     }
    //     set
    //     {
    //         door4DissolveNum = value;
    //         TeleportColliderDoorsDictionary[3].SetDissolveNum(door4DissolveNum);
    //     }
    // }

    public void IntroduceDeviceDone()
    {
        RobotObj.Instance.SpeechText("哇！小朋友，你太棒啦！你已经学会了手柄的基础操作，接下来，让我带你去探索更多神奇的地方，开始我们的冒险吧",(() =>
        {
            // ShowDoor(0);
            //移动进入第一个门

            RobotObj.Instance.SetDestination( GetPoint(2).position,(() =>
            {
                Vector3 playerPos = PlayerObj.Instance.transform.position;
                playerPos.y = RobotObj.Instance.transform.position.y;
                RobotObj.Instance.transform.DORotateQuaternion(
                    Quaternion.LookRotation(playerPos -
                                            RobotObj.Instance.transform.position), 1f);
                TimerMgr.Instance.StartTimer(15, (() =>
                {
                    //播放超时音效
                }));
            }));
            NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, GetPoint(1).position);
        }));
    }

    // public void ShowDoor(int targetId)
    // {
    //     TeleportColliderDoorsDictionary[targetId].SetDissolveNum(1);
    //     TeleportColliderDoorsDictionary[targetId].gameObject.SetActive(true);
    //     switch (targetId)
    //     {
    //         case 0:
    //             DOTween.To(() => Door1DissolveProcess, (x => Door1DissolveProcess = x), 0, 2);
    //             break;
    //         case 1:
    //             DOTween.To(() => Door2DissolveProcess, (x => Door1DissolveProcess = x), 0, 2);
    //             break;
    //         case 2:
    //             DOTween.To(() => Door3DissolveProcess, (x => Door1DissolveProcess = x), 0, 2);
    //             break;
    //         case 3:
    //             DOTween.To(() => Door4DissolveProcess, (x => Door1DissolveProcess = x), 0, 2);
    //             break;
    //     }
    //
    // }

}
