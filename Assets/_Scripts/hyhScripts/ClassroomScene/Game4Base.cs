using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Game4Base : MonoBehaviour
{
    public XRGrabInteractable Interactable;

    public Transform robotPos;
    // Start is called before the first frame update
    void Start()
    {
        // Interactable.selectEntered.AddListener((arg0 =>
        // {
        //     RobotObj.Instance.SetDestination(robotPos.position,(() =>
        //     {
        //         Vector3 playerPos = PlayerObj.Instance.transform.position;
        //         playerPos.y = RobotObj.Instance.transform.position.y;
        //         RobotObj.Instance.transform.LookAt(playerPos);
        //     }));
        // }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
