using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivePreviewCamera : MonoBehaviour
{
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (PlayerObj.Instance != null)
        {
            Transform playerTrans = PlayerObj.Instance.transform;

            transform.position = playerTrans.position + playerTrans.forward * -0.1f;
            transform.LookAt(playerTrans.position + playerTrans.forward * 2f);
        }
    }
}
