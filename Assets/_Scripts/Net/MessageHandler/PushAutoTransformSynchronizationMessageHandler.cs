
using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using UnityEngine;

public class PushAutoTransformSynchronizationMessageHandler: Message<PushAutoTransformSynchronizationMessage>
{
    protected override async FTask Run(Session session, PushAutoTransformSynchronizationMessage message)
    {
        Router.Instance.RouteNetworkMessage(message.uuid,message);
        await FTask.CompletedTask;
    }
}