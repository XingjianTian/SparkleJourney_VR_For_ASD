
using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using UnityEngine;

public class PushRemoveClientMessageHandler: Message<PushRemoveClientMessage>
{
    protected override async FTask Run(Session session, PushRemoveClientMessage message)
    {
        int playerUuid = message.playerIndex * 10 + 0;
        GameObject go = (Router.Instance.GetRoutedObject(playerUuid) as AutoTransformSynchronization).gameObject;
        GameObject.Destroy(go.transform.parent.gameObject);
    }
}
