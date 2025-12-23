using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Collider))]
public class TeleportColliderObj : MonoBehaviour
{

    public int targetId;



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        switch (targetId)
        {
            case 0:
                SceneManager.LoadSceneAsync("Room_Unity");
                break;
            case 1:
                SceneManager.LoadSceneAsync("classroom");
                break;
            case 2:
                SceneManager.LoadSceneAsync("LowPolyMegapolis_Demo");
                break;
        }
    }
}
