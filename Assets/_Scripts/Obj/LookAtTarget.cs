using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Vector3 lookTarget;
    // Update is called once per frame
    void Update()
    {
        lookTarget = target.position;
        lookTarget.y = transform.position.y;
        transform.rotation = Quaternion.LookRotation(lookTarget - transform.position) * Quaternion.Euler(offset);
    }
}
