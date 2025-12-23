using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class Game2Base : MonoBehaviour
{
    [Header("玩家头部")]
    public Transform playerHead;
    [Header("XRSocket组件")]
    public XRSocketInteractor[] socketList;
    [Header("机器人站位")]
    public Transform robotPosition;
    [Header("拼图出现位置")]
    public Transform points;

    private GameObject currentpuzzle;
    private GameObject currentsocket;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RobotObj.Instance.SpeechText("你想玩拼图嘛,那我们开始吧",GameLevel1Start);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Clear();
        }
    }

    void Clear()
    {
        Destroy(currentpuzzle);
        Destroy(currentsocket);
        socketList = new XRSocketInteractor[]{};
    }
    

    


    //第一关
    void GameLevel1Start()
    {
        RobotObj.Instance.SetDestination(robotPosition.position,(() =>
        {
            Vector3 playerPos = PlayerObj.Instance.transform.position;
            playerPos.y = RobotObj.Instance.transform.position.y;
            RobotObj.Instance.transform.LookAt(playerPos);
        }));
        currentpuzzle = Instantiate(Resources.Load<GameObject>("Prefabs/hyh/Game2/Level1/puzzle1"),transform);
        currentsocket = Instantiate(Resources.Load<GameObject>("Prefabs/hyh/Game2/Level1/puzzle1Socket"),transform);
        socketList = new XRSocketInteractor[9];
        socketList = currentsocket.GetComponentsInChildren<XRSocketInteractor>();
        Transform[] children = currentpuzzle.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].position = points.position + Vector3.up * 0.1f * i;
        }

        int num = 0;
        foreach (var itme in socketList)
        {
            itme.selectEntered.AddListener((obj) =>
            {
                if (obj.interactableObject.transform.GetComponent<ID>().id == itme.GetComponent<ID>().id)
                {
                    num++;

                    if (num == 9)
                    {
                        
                        RobotObj.Instance.SpeechText("太棒啦，你完成了拼图，让我们增加一点难度", () =>
                        {
                            Clear();
                            GameLevel2Start();
                        });
                    }
                }
            });
            itme.selectExited.AddListener((obj) =>
            {
                if (obj.interactableObject.transform.GetComponent<ID>().id == itme.GetComponent<ID>().id)
                {
                    num--;
                }
            });
        }
    }
    //第二关
    void GameLevel2Start()
    {
        currentpuzzle = Instantiate(Resources.Load<GameObject>("Prefabs/hyh/Game2/Level2/puzzle2"),transform);
        currentsocket = Instantiate(Resources.Load<GameObject>("Prefabs/hyh/Game2/Level2/puzzle2Socket"),transform);
        socketList = new XRSocketInteractor[16];
        socketList = currentsocket.GetComponentsInChildren<XRSocketInteractor>();
        Transform[] children = currentpuzzle.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].position = points.position + Vector3.up * 0.1f * i;
        }
        int num = 0;
        foreach (var itme in socketList)
        {
            itme.selectEntered.AddListener((obj) =>
            {
                if (obj.interactableObject.transform.GetComponent<ID>().id == itme.GetComponent<ID>().id)
                {
                    num++;
                    if (num == 16)
                    {
                        RobotObj.Instance.SpeechText("太棒啦，你完成了拼图，我们去玩其他游戏吧", StopAllCoroutines);
                    }
                }
            });
            itme.selectExited.AddListener((obj) =>
            {
                if (obj.interactableObject.transform.GetComponent<ID>().id == itme.GetComponent<ID>().id)
                {
                    num--;
                }
            });
        }
    }
}
