using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedOhterBodyForward : MonoBehaviour
{
    public Transform netBody;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = netBody.position;
        pos.y = 1.24f -0.37f;
        transform.position = pos;
        Vector3 rot = netBody.rotation.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.Euler(rot);
    }
}
