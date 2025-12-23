
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Fantasy;
using Fantasy.Network;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

public class GameInitlizer: BaseMgr<GameInitlizer>
{
    private GameInitlizer()
    {
    }

    //初始化玩家
    public async Task InitOtherPlayer()
    {
        GetAllClientRequest request = new GetAllClientRequest();
        FantasyNetMgr.Instance.Call(request,  (async res =>
        {
            var response = res as GetAllClientResponse;
            List<int> playerIndexList = response.playerIndexList;
            Debug.Log($"服务端人数{playerIndexList.Count}");
            foreach (int playerIndex in playerIndexList)
            {
                Debug.Log(playerIndex + "------"+await FantasyNetMgr.Instance.GetLocalPlayerIndex());
                if (playerIndex == await FantasyNetMgr.Instance.GetLocalPlayerIndex())
                {
                    continue;
                }
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
        } ));

    }

    public async void InitLocalPlayer(int playerIndex)
    {
        GameObject goPre = Resources.Load<GameObject>("Prefabs/wjw/LocalPlayer");
        GameObject go = GameObject.Instantiate(goPre);
        PlayerBuilding building = go.GetComponent<PlayerBuilding>();
        AutoTransformSynchronization autoTransform =building.body.AddComponent<AutoTransformSynchronization>();
        autoTransform.uuid = playerIndex * 10 + 0;
        int localPlayerIndex = await FantasyNetMgr.Instance.GetLocalPlayerIndex();
        autoTransform.SetOwnerWithLocalPlayer(localPlayerIndex);
        Router.Instance.RegistryNetworkComponent(autoTransform);
        autoTransform =building.leftController.AddComponent<AutoTransformSynchronization>();
        autoTransform.uuid = playerIndex * 10 + 1;
        autoTransform.SetOwnerWithLocalPlayer(localPlayerIndex);
        Router.Instance.RegistryNetworkComponent(autoTransform);
        autoTransform =building.rightController.AddComponent<AutoTransformSynchronization>();
        autoTransform.uuid = playerIndex * 10 + 2;
        autoTransform.SetOwnerWithLocalPlayer(localPlayerIndex);
        Router.Instance.RegistryNetworkComponent(autoTransform);
    }

    //同步所有要同步物体的位置
    public async void UpdateSceneOnSwaped()
    {
        FantasyNetMgr.Instance.Call(new GetAllSubmitAutoTransformSynchronizationRequest() { socketClinetUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID() },(
            res =>
            {
                GetAllSubmitAutoTransformSynchronizationResponse response =
                    res as GetAllSubmitAutoTransformSynchronizationResponse;
                List<SubmitAutoTransformSynchronizationMessage> data = response.messageList;
                foreach (var item in data)
                {
                    PushAutoTransformSynchronizationMessage message = new PushAutoTransformSynchronizationMessage();
                    message.uuid = item.uuid;
                    message.position = item.position;
                    message.rotation = item.rotation;
                    message.scale = item.scale;
                    Router.Instance.RouteNetworkMessage(message.uuid,message);
                }

                List<BaseNetworkComponent> baseNetworkComponentList = Router.Instance.GetInteralComponentList();
                foreach (var component in baseNetworkComponentList)
                {
                    if (component.GetType() == typeof(AutoTransformSynchronization))
                    {
                        MethodInfo info= component.GetType().GetMethod("InitOwner");
                        info.Invoke(component,null);
                    }
                }

            }));

    }
}
