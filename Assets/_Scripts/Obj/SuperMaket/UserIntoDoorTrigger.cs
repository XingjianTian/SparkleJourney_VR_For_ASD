using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserIntoDoorTrigger : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool isDone;
    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        if (!isDone&&other.CompareTag("Player"))
        {
            isDone = true;
            //店员进行问好
            ClerkObj.Instance.DoWelcome();
            ClerkObj.Instance.SpeechText("欢迎光临",(clip =>
            {
                MusicMgr.Instance.effCore.PlaySound(clip);
            }));
            SuperMarketMgr.Instance.TakeMarketCar();
        }
    }
}
