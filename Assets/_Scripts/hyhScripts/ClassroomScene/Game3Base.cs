using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class Game3Base : MonoBehaviour
{
    private static Game3Base instance;
    public static Game3Base Instance => instance;
    private void Awake()
    {
        instance = this;
    }

    public Button again;
    public XRSocketInteractor[] socketList;
    public Game3ExampleCube[] gameObjList;
    public int num;
    public int currentShowObj;
    [Header("初始方块预制体")]
    public GameObject prefab;

    public Transform robotPos;
    void Start()
    {
        again.onClick.AddListener(() =>
        {
            RobotObj.Instance.SetDestination(robotPos.position,(() =>
            {
                Vector3 playerPos = PlayerObj.Instance.transform.position;
                playerPos.y = RobotObj.Instance.transform.position.y;
                RobotObj.Instance.transform.LookAt(playerPos);
            }));
            num = 0;
            currentShowObj = 6;
            Instantiate(prefab,transform);
            for (int i = 0; i < gameObjList.Length; i++)
            {
                gameObjList[i].UpdateAgain();
                socketList[i].GetComponent<Game3Socket>().UpdateVis();
                if (socketList[i].hasSelection){Destroy(socketList[i].selectTarget.gameObject);}
            }
        });
    }
}
