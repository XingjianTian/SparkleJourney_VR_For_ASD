using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace _Scripts.Net.MessageHandler
{
    public class PushNetRigidbodySynchronizationMessageHandler: Message<PushNetRigidbodySynchronizationMessage>
    {
        protected override async FTask Run(Session session, PushNetRigidbodySynchronizationMessage message)
        {
            Router.Instance.RouteNetworkMessage(message.uuid,message);
            await FTask.CompletedTask;
        }
    }
}