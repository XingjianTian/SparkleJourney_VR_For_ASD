using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class BallObj : MonoBehaviour
{
    private XRSimpleInteractable _xrSimpleInteractable;

    private bool isBack;
    // Start is called before the first frame update
    void Start()
    {
        _xrSimpleInteractable = GetComponent<XRSimpleInteractable>();
        _xrSimpleInteractable.hoverEntered.AddListener((hoverEnterEventArgs =>
        {
                if (isBack)
                {
                    return;
                }

                isBack = true;
               BallController.Instance.ReciveBall();
               TimerMgr.Instance.StartTimer(1.6f, (() =>
               {
                   BallController.Instance.ThrowBall();
                   isBack = false;
               }));
        }));
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            BallController.Instance.ThrowBall();
        }
    }
}
