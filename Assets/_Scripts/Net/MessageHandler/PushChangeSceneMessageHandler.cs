
using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Net.MessageHandler
{
    public class PushChangeSceneMessageHandler: Message<PushChangeSceneMessage>
    {
        protected async override FTask Run(Session session, PushChangeSceneMessage message)
        {
            if (SceneManager.GetActiveScene().name == message.scene_name)
            {
                Debug.LogWarning($"切换到场景:{message.scene_name} 失败，当前就是 {message.scene_name} 场景");
                return;
            }
            SceneManager.LoadSceneAsync(message.scene_name);
            await FTask.CompletedTask;
        }
    }
}