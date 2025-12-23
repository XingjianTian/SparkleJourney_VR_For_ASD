using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GoodObj : MonoBehaviour
{
    public string name;
    public float price;

    private XRGrabInteractable interactable;

    private Rigidbody rig;
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<XRGrabInteractable>();
        rig = GetComponent<Rigidbody>();
        rig.constraints = RigidbodyConstraints.FreezeAll;
        interactable.selectEntered.AddListener((v =>
        {
            rig.constraints = RigidbodyConstraints.None;
        }));
        interactable.selectExited.AddListener((v =>
        {
            rig.constraints =  RigidbodyConstraints.FreezePositionX |
                              RigidbodyConstraints.FreezePositionZ;
        }));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
