using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

public class Game3ExampleCube : MonoBehaviour
{
    private Color[] Randomcolors;
    public XRSocketInteractor socket;
    public Game3ExampleCube downObj;
    //是否跳过隐藏
    public bool isReturn;
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        socket.selectEntered.AddListener((obj) =>
        {
            if (obj.interactableObject.transform.GetComponent<MeshRenderer>().material.color ==
                GetComponent<MeshRenderer>().material.color)
            {
                Game3Base.Instance.num++;
                if (Game3Base.Instance.num == Game3Base.Instance.currentShowObj)
                {
                    RobotObj.Instance.SpeechText("太棒啦，你搭的真好");
                }
            }
        });
        socket.selectExited.AddListener((obj) =>
        {
            if (obj.interactableObject.transform.GetComponent<MeshRenderer>().material.color ==
                GetComponent<MeshRenderer>().material.color)
            {
                Game3Base.Instance.num--;
            }
        });
    }

    public void UpdateAgain()
    {
        Game3Socket socketContrul = socket.GetComponent<Game3Socket>();
        gameObject.SetActive(true);
        socket.gameObject.SetActive(true);
        socketContrul.isAllow = false;
        Randomcolors = new[]
        {
            Color.black, 
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            Color.gray,
            Color.white,
        };
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material.color = Randomcolors[Random.Range(0, Randomcolors.Length)];

        if (isReturn)
        {
            socketContrul.isAllow = true;
            return;
        }
        
        int rannum = Random.Range(0, 101);
        if (rannum < 50)
        {
            gameObject.SetActive(false);
            socket.gameObject.SetActive(false);
            Game3Base.Instance.currentShowObj--;
            print("被隐藏的对应物品："+gameObject.name);
            print(Game3Base.Instance.currentShowObj);
        }
        else
        {
            if (downObj != null) downObj.isReturn = true;
        }
    }
}
