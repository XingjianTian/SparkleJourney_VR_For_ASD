using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using UnityEngine;

namespace _Scripts.Net.LivePreview
{
    public class PushStartGamePreviewMessageHandler: Message<PushStartGamePreviewMessage>
    {
        protected override async FTask Run(Session session, PushStartGamePreviewMessage message)
        {
            Debug.Log("开始livePreview推送");
            LivePreviewNetMgr.Instance.StartLivePreview();
            await FTask.CompletedTask;
        }
    }
}