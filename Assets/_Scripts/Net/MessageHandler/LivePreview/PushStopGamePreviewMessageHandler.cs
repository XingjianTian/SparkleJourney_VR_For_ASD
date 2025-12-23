using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using UnityEngine;

namespace _Scripts.Net.LivePreview
{
    public class PushStopGamePreviewMessageHandler: Message<PushStopGamePreviewMessage>
    {
        protected override async FTask Run(Session session, PushStopGamePreviewMessage message)
        {
            Debug.Log("关闭livePreview推送");
            LivePreviewNetMgr.Instance.StopLivePreview();
            await FTask.CompletedTask;
        }
    }
}