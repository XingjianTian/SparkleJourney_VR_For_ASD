using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDelayDone : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("DelayDone",3);
    }


    private void DelayDone()
    {
        CachePool.Instance.Push(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
