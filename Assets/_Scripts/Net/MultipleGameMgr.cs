using System;
using System.Collections;
using System.Collections.Generic;
using Fantasy;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultipleGameMgr : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        FantasyNetMgr.Instance.InitMultiplePlayerScene();
        string socketClientUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID();
        Debug.Log($"加入多人联机");
        FantasyNetMgr.Instance.Send(new SubmitClientJoin2MultipleGameMessage()
        {
            socketClinetUUID =socketClientUUID
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    private async void OnDestroy()
    {
        string socketClientUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID();
        Debug.Log($"退出多人联机");
        FantasyNetMgr.Instance.Send(new SubmitClientExit2MultipleGameMessage()
        {
            socketClinetUUID =socketClientUUID
        });
    }
}
