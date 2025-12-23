using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;

public class Follow2Camera : MonoBehaviour
{
    public Transform TargetTransform;

    public float yOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = TargetTransform.position - Vector3.up * yOffset;
        Vector3 targetRot = TargetTransform.rotation.eulerAngles;
        targetRot.x = 0;
        targetRot.z = 0;
        transform.rotation = Quaternion.Euler(targetRot);
    }
}
