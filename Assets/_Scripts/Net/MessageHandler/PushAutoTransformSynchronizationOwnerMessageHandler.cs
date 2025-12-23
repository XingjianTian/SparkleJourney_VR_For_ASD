using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace _Scripts.Net.MessageHandler
{
    public class PushAutoTransformSynchronizationOwnerMessageHandler: Message<PushAutoTransformSynchronizationOwnerMessage>
    {
        protected override async FTask Run(Session session, PushAutoTransformSynchronizationOwnerMessage message)
        {
            AutoTransformSynchronization autoTransformSynchronization= Router.Instance.GetRoutedObject(message.uuid) as AutoTransformSynchronization;
            autoTransformSynchronization.SetOwner(message.playerIndex);
        }
    }
}