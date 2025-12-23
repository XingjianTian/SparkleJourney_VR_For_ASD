using System;
using Fantasy.Network;
using Fantasy.Network.Interface;
using UnityEngine;

public class BaseNetworkComponent: MonoBehaviour
{
    public int uuid;
    public bool autoRegistry = false;

    public virtual void HandleMessage(IMessage message)
    {

    }

    protected void SendProtoMessage(IMessage message)
    {
        FantasyNetMgr.Instance.SendMessage(message);
    }
    protected virtual void Awake()
    {
        if (autoRegistry)
        {
            Router.Instance.RegistryNetworkComponent(this);
        }
    }
}