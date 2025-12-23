using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CarObj : MonoBehaviour
{
    public float degreen = 1;
    private XRSimpleInteractable _xrSimpleInteractable;

    private Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        _xrSimpleInteractable = GetComponent<XRSimpleInteractable>();
        _rigidbody = transform.GetComponent<Rigidbody>();
        _xrSimpleInteractable.hoverEntered.AddListener((hoverEnterEventArgs =>
        {
            print(_rigidbody);
            _rigidbody.AddForce(transform.forward  * degreen,ForceMode.Impulse);
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
