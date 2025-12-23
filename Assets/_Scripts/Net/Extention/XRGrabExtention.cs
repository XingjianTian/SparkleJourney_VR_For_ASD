using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable),typeof(AutoTransformSynchronization),typeof(NetRigidbodyComponent))]
public class XRGrabExtention : MonoBehaviour
{

    private XRGrabInteractable _interactable;

    private AutoTransformSynchronization _autoTransformSynchronization;

    private NetRigidbodyComponent _netRigidbodyComponent;
    // Start is called before the first frame update
    void Start()
    {
        _interactable = GetComponent<XRGrabInteractable>();
        _autoTransformSynchronization = GetComponent<AutoTransformSynchronization>();
        _netRigidbodyComponent = GetComponent<NetRigidbodyComponent>();
        _interactable.selectEntered.AddListener((selectEnterEventArgs =>
        {
            _autoTransformSynchronization.SetOwnerWithLocalPlayer();
            _netRigidbodyComponent.isKinematic = true;
        }));
        _interactable.selectExited.AddListener((selectExitEventArgs =>
        {
            _netRigidbodyComponent.isKinematic = false;
        }));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
