using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandTargetObj : MonoBehaviour
{

    private XRSimpleInteractable _xrSimpleInteractable;

    private Vector3 originPos;
    // Start is called before the first frame update
    void Start()
    {
        originPos = transform.position;
        _xrSimpleInteractable = GetComponent<XRSimpleInteractable>();

        _xrSimpleInteractable.selectEntered.AddListener((selectEnterEventArgs =>
        {
            target = (selectEnterEventArgs.interactorObject as XRBaseInteractor).transform;
            isFollow = true;
        }));
        _xrSimpleInteractable.selectExited.AddListener((selectEnterEventArgs =>
        {
            isFollow = false;
            transform.DOMove(originPos, 2);
        }));
    }

    private bool isFollow;

    private Transform target;
    // Update is called once per frame
    void Update()
    {
        if (isFollow)
        {
            transform.position = target.position;
        }
    }
}
