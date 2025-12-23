
using System;
using System.Collections;
using Fantasy;
using Fantasy.Network.Interface;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

public class AutoTransformSynchronization: BaseNetworkComponent
{


    private int ownerPlayerIndex;

    public void SetOwner(int playerIndex)
    {
        ownerPlayerIndex = playerIndex;
    }


    public async void  SetOwnerWithLocalPlayer(int playerIndex)
    {
        SubmitAutoTransformSynchronizationOwnerMessage message = new SubmitAutoTransformSynchronizationOwnerMessage();
        ownerPlayerIndex = playerIndex;
        message.uuid = uuid;
        message.socketClinetUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID();
        message.playerIndex = playerIndex;
        FantasyNetMgr.Instance.Send(message);
    }
    public async void  SetOwnerWithLocalPlayer()
    {
        SubmitAutoTransformSynchronizationOwnerMessage message = new SubmitAutoTransformSynchronizationOwnerMessage();
        ownerPlayerIndex = await FantasyNetMgr.Instance.GetLocalPlayerIndex();
        message.uuid = uuid;
        message.socketClinetUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID();
        message.playerIndex = await FantasyNetMgr.Instance.GetLocalPlayerIndex();
        FantasyNetMgr.Instance.Send(message);
    }
    public override void HandleMessage(IMessage message)
    {
        base.HandleMessage(message);
        PushAutoTransformSynchronizationMessage msg = message as PushAutoTransformSynchronizationMessage;
        transform.position = new Vector3(msg.position.x, msg.position.y, msg.position.z);
        transform.rotation = Quaternion.Euler(new Vector3(msg.rotation.x, msg.rotation.y, msg.rotation.z));
        transform.localScale = new Vector3(msg.scale.x, msg.scale.y, msg.scale.z);
        tempPosition = new Vector3(msg.position.x, msg.position.y, msg.position.z);
        tempRotation = new Vector3(msg.rotation.x, msg.rotation.y, msg.rotation.z);
        tempScale = new Vector3(msg.scale.x, msg.scale.y, msg.scale.z);
    }

    protected virtual void Start()
    {
        StartCoroutine(HandlePositionUpdate());
    }

    // protected virtual async void FixedUpdate()
    // {
    //     if (ownerPlayerIndex == await FantasyNetMgr.Instance.GetLocalPlayerIndex() && IsShouldUpdate())
    //     {
    //         SendUpdateMessage();
    //     }
    // }

    IEnumerator  HandlePositionUpdate()
    {

        WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
        while (true)
        {
            HandlePositionInteral();

            yield return waitForSeconds;
        }
    }

    private async void HandlePositionInteral()
    {
        if (ownerPlayerIndex == await FantasyNetMgr.Instance.GetLocalPlayerIndex() && IsShouldUpdate())
        {
            SendUpdateMessage();
        }
    }

    public async void InitOwner()
    {
        if (ownerPlayerIndex == await FantasyNetMgr.Instance.GetLocalPlayerIndex())
        {
            return;
        }
        GetAutoTransformSynchronizationOwnerRequest request = new GetAutoTransformSynchronizationOwnerRequest();
        request.uuid = this.uuid;
        FantasyNetMgr.Instance.Call(request,(
            response =>
            {
                ownerPlayerIndex = (response as GetAutoTransformSynchronizationOwnerResponse).playerIndex;
            } ));
    }
    private Vector3 tempPosition;
    private Vector3 tempRotation;
    private Vector3 tempScale;
    protected virtual void SendUpdateMessage()
    {
        if (FantasyNetMgr.Instance == null ||  !FantasyNetMgr.isConnected)
        {
            return;
        }
        SubmitAutoTransformSynchronizationMessage message = new SubmitAutoTransformSynchronizationMessage();
        message.uuid = this.uuid;
        Vector3 position = transform.position;
        Vector3 rotation = transform.rotation.eulerAngles;
        Vector3 scale = transform.localScale;
        message.position = new Fantasy.Vector3(){x = position.x,y = position.y,z = position.z};
        message.rotation = new Fantasy.Vector3(){x = rotation.x,y = rotation.y,z = rotation.z};
        message.scale = new Fantasy.Vector3(){x = scale.x,y = scale.y,z = scale.z};

        SendProtoMessage(message);
    }

    private bool IsShouldUpdate()
    {
        Vector3 pos = transform.position;
        Vector3 rot = transform.rotation.eulerAngles;
        Vector3 scale = transform.localScale;
        if (pos != tempPosition || rot != tempRotation || scale != tempScale)
        {
            tempPosition = pos;
            tempRotation = rot;
            tempScale = scale;
            return true;
        }

        return false;
    }
}