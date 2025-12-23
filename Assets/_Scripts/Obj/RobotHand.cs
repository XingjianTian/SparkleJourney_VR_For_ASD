using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR.Interaction.Toolkit;

public class RobotHand : MonoBehaviour
{
    private XRGrabInteractable xrGrabInteractable;

    public Rig rig;

    public Transform handTarget;


    private bool isSelected;

    private Vector3 originPos;
    // Start is called before the first frame update
    void Start()
    {
        originPos = handTarget.position;
        xrGrabInteractable = GetComponent<XRGrabInteractable>();

        xrGrabInteractable.selectEntered.AddListener((arg0 =>
        {
            rig.weight = 1;
            isSelected = true;
        } ));

        xrGrabInteractable.selectExited.AddListener((arg0 =>
        {
            rig.weight = 0;
            handTarget.position = originPos;
        }));

    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            handTarget.position = transform.position;
            handTarget.rotation = transform.rotation;
        }
    }
}
