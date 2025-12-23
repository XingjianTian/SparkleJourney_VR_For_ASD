//这里根据网络组件来分类

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fantasy.Network.Interface;
using UnityEngine;
using UnityEngine.AI;


public class Router: BaseMgr<Router>
{
    private Router()
    {

    }
    private Dictionary<int, BaseNetworkComponent> interalRouteDic = new Dictionary<int, BaseNetworkComponent>();

    public List<BaseNetworkComponent> GetInteralComponentList()
    {
        return interalRouteDic.Values.ToList();
    }

    //在路由注册网络组件，以便统一接受消息，然后分发给对应的组件
    public void RegistryNetworkComponent(BaseNetworkComponent component)
    {
        if (!interalRouteDic.ContainsKey(component.uuid))
        {
            interalRouteDic.Add(component.uuid, component);
        }
        else
        {
            interalRouteDic[component.uuid] = component;
        }
    }

    public void RouteNetworkMessage(int uuid,IMessage message)
    {
        if (!interalRouteDic.ContainsKey(uuid))
        {
            Debug.Log($"无法路由:{uuid}");
            return;
        }
        interalRouteDic[uuid].HandleMessage(message);
    }

    public Object GetRoutedObject(int uuid)
    {
        if (!interalRouteDic.ContainsKey(uuid))
        {
            Debug.Log($"无法GetRoutedObject:{uuid}");
            return null;
        }

        return interalRouteDic[uuid];
    }
}
