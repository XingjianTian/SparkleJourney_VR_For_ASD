using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Fantasy;
using Fantasy.Network;
using Fantasy.Network.Interface;
using UnityEngine;
using UnityEngine.Events;

public class FantasyNetMgr: BaseMgr<FantasyNetMgr>
{
    private static Scene worldScene;

    private static object lockobj;



    private FantasyNetMgr()
    {
        InitFantasy();
        // MonoMgr.Instance.AddApplicationQuitEvent(OnApplicationQuit);
    }
    private string socketClinetUUID
    {
        get; set;
    }


    // void OnApplicationQuit()
    // {
    //     if (socketClinetUUID != null)
    //     {
    //         Debug.Log($"主动断开连接");
    //         FantasyNetMgr.Instance.Send(new RemoveClientMessage()
    //         {
    //             socketClinetUUID =FantasyNetMgr.Instance.socketClinetUUID
    //         });
    //     }
    //     // _session.Dispose();
    //     // isConnected = false;
    // }

    public static bool isConnected;
    private static bool isInitFramework;

    private async void InitFantasy()
    {
        if (!isInitFramework)
        {
            //初始化fantasy
            Fantasy.Platform.Unity.Entry.Initialize(GetType().Assembly);
            worldScene = await Fantasy.Platform.Unity.Entry.CreateScene();
            isInitFramework = true;
        }


        Connect2Server();
    }

    private int localPlayerIndex;
    private TaskCompletionSource<string> _taskCompletionSource = new TaskCompletionSource<string>();
    public async Task<string> GetSocketClinetUUID()
    {
       return await _taskCompletionSource.Task;
    }
    private TaskCompletionSource<int> _taskCompletionSourceForPlayerIndex = new TaskCompletionSource<int>();
    public async Task<int> GetLocalPlayerIndex()
    {
        return await _taskCompletionSourceForPlayerIndex.Task;

    }
    private void Connect2Server()
    {
        _session = worldScene.Connect("120.55.71.69:8888", NetworkProtocolType.KCP, (async () =>
        {
            Debug.Log("服务器连接成功");
            _session.AddComponent<SessionHeartbeatComponent>().Start(1000,1000000000);
            RegistryClientResponse response = (RegistryClientResponse)await _session.Call(new RegistryClientRequest());
            socketClinetUUID = response.socketClinetUUID;
            _taskCompletionSource.SetResult(socketClinetUUID);
            Debug.Log($"获取到客户端uuid:{socketClinetUUID}");
            isConnected = true;
            localPlayerIndex = response.currentIndex;
            _taskCompletionSourceForPlayerIndex.SetResult(localPlayerIndex);
            MonoMgr.Instance.StartCoroutine(HandleOverStockMessge());
        }), (() =>
        {
            Debug.Log("连接服务器失败");
        }), (() =>
        {

            Debug.Log("与服务器断开连接");
            isConnected = false;

            // Connect2Server();
        }), false, 5000);
    }

    public async void InitMultiplePlayerScene()
    {
        await GetSocketClinetUUID();
        await GameInitlizer.Instance.InitOtherPlayer();
        GameInitlizer.Instance.InitLocalPlayer(await GetLocalPlayerIndex());
        GameInitlizer.Instance.UpdateSceneOnSwaped();

    }

    public void SendMessage(IMessage message)
    {
        PropertyInfo info = message.GetType().GetProperty("socketClinetUUID");
        if (info == null)
        {
            Debug.LogError($"当前Messge:{message.GetType().Name}没有socketClinetUUID属性");
            return;
        }
        info.SetValue(message,socketClinetUUID);
        if (_session == null)
        {
            OverStockMessge msg = new OverStockMessge(message, MsgType.IMessage);
            overstockMessageQueue.Enqueue(msg);
            return;
        }

        if (socketClinetUUID == "")
        {
            Debug.LogError($"无法与服务端进行通信socketClinetUUID 为 空");
            return;
        }
        _session.Send(message);
    }

    private Session _session;
    IEnumerator HandleOverStockMessge()
    {
        while (overstockMessageQueue.Count > 0)
        {
            OverStockMessge overStockMessge = overstockMessageQueue.Dequeue();
            if (overStockMessge.msgType == MsgType.IMessage)
            {
                Send(overStockMessge.msg);
            } else if (overStockMessge.msgType == MsgType.IRequest)
            {
                Call(overStockMessge.msg as IRequest,overStockMessge.callback);
            }

            yield return null;
        }
    }

    private Queue<OverStockMessge> overstockMessageQueue = new Queue<OverStockMessge>();

    enum MsgType
    {
        IMessage,
        IRequest
    }

    class OverStockMessge
    {
        public IMessage msg;
        public MsgType msgType;
        public UnityAction<IResponse> callback;

        public OverStockMessge(IMessage msg, MsgType type,UnityAction<IResponse> callback= null)
        {
            this.msg = msg;
            this.msgType = type;
            this.callback = callback;
        }
    }

    public void Send(IMessage message)
    {
        if (!isConnected)
        {
            overstockMessageQueue.Enqueue(new OverStockMessge(message,MsgType.IMessage));
            return;
        }
        _session.Send(message);
    }

    public async void Call(IRequest request,UnityAction<IResponse> callback = null)
    {
        if (!isConnected)
        {
            overstockMessageQueue.Enqueue(new OverStockMessge(request,MsgType.IRequest,callback));
            return;
        }
        IResponse response = await _session.Call(request);
        callback?.Invoke(response);
    }
}
