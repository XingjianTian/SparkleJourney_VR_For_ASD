using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
[Serializable]
public enum SceneName
{
    Road,
    SuperMarket,
    Park
}

public class SuperMarketMgr : MonoSingleton<SuperMarketMgr>
{
    public  SceneName currentScene = SceneName.Road;

    [Header("SuperMarket")]
    public Transform takeCarPos;

    public Transform marketCar;
    public Rig rig;
    public GameObject goodPanel;

    [Header("InputSystem")] public InputActionReference inputCalculateBtn;

    private NavMeshAgent robotAgent;
    // Start is called before the first frame update
    void Start()
    {
        robotAgent = RobotObj.Instance.GetComponent<NavMeshAgent>();
        robotAgent.stoppingDistance = 0.5f;
        inputCalculateBtn.action.performed += ReleaseMarketCar;

    }



    private bool enableMarketInteralNav;
    // Update is called once per frame
    void Update()
    {
        switch (currentScene)
        {
            case SceneName.Road:
                RobotObj.Instance.SetDestination(PlayerObj.Instance.transform.position + new Vector3(1,0,1));
                break;
            case SceneName.SuperMarket:
                if (enableMarketInteralNav)
                {
                    RobotObj.Instance.SetDestination(PlayerObj.Instance.transform.position);
                }
                break;
            case SceneName.Park:

                break;
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            TakeMarketCar();
        }

    }

    public void TakeMarketCar()
    {
        currentScene = SceneName.SuperMarket;
        robotAgent.stoppingDistance = 0.05f;
        RobotObj.Instance.navTargetDistance = 0.3f;
        RobotObj.Instance.SetDestination(takeCarPos.position,(() =>
        {
            RobotObj.Instance.transform.position = takeCarPos.position;
            enableMarketInteralNav = true;
            rig.weight = 1;
            robotAgent.stoppingDistance = 1.6f;
            marketCar.parent = RobotObj.Instance.transform;
            TimerMgr.Instance.StartTimer(0.3f, () =>
            {
                marketCar.localScale = new Vector3(0.003f, 0.006f, 0.228f);
                marketCar.localRotation = Quaternion.Euler(new Vector3(-89.98f, 0, 90.167f));
                marketCar.localScale = new Vector3(0.006863901f, 0.007808374f, 0.006138799f);
            });

        }));
    }

    public bool IsInCalculateArea;
    public void ReleaseMarketCar(InputAction.CallbackContext context)
    {
        if (!IsInCalculateArea)
        {
            return;
        }
        inputCalculateBtn.action.performed -= ReleaseMarketCar;
        rig.weight = 0;
        enableMarketInteralNav = false;
        currentScene = SceneName.Road;
        robotAgent.stoppingDistance = 0.5f;
        marketCar.parent = null;
        if (goodPanel!=null)
        {
            goodPanel.SetActive(false);
        }
        ClerkObj.Instance.SpeechText("欢迎下次光临",(clip =>
        {
            MusicMgr.Instance.effCore.PlaySound(clip);
        }));
    }
}
