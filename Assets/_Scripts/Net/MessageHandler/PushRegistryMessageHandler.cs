
using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Unity.VisualScripting;
using UnityEngine;

public class PushRegistryMessageHandler: Message<PushRegistryMessage>
{
    protected override async FTask Run(Session session, PushRegistryMessage message)
    {
        int playerIndex = message.playerIndex;
        GameObject goPre = Resources.Load<GameObject>("Prefabs/wjw/OtherPlayer");
        GameObject go = GameObject.Instantiate(goPre);
        PlayerBuilding building = go.GetComponent<PlayerBuilding>();
        AutoTransformSynchronization autoTransform =building.body.AddComponent<AutoTransformSynchronization>();
        autoTransform.uuid = playerIndex * 10 + 0;
        Router.Instance.RegistryNetworkComponent(autoTransform);
        autoTransform =building.leftController.AddComponent<AutoTransformSynchronization>();
        autoTransform.uuid = playerIndex * 10 + 1;
        Router.Instance.RegistryNetworkComponent(autoTransform);
        autoTransform =building.rightController.AddComponent<AutoTransformSynchronization>();
        autoTransform.uuid = playerIndex * 10 + 2;
        Router.Instance.RegistryNetworkComponent(autoTransform);
    }
}
