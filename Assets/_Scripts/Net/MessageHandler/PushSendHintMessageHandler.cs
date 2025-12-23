using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using UnityEngine;

namespace _Scripts.Net.MessageHandler
{
    public class PushSendHintMessageHandler: Message<PushSendHintMessage>
    {
        private AudioSource _source;

        protected override async FTask Run(Session session, PushSendHintMessage message)
        {
            if (_source == null)
            {
                GameObject go = new GameObject("TeacherHintAudioSource");
                GameObject.DontDestroyOnLoad(go);
                _source = go.AddComponent<AudioSource>();
            }
            await Text2SpeechHandle.Instance.Send(message.hint_message, (clip =>
            {
                _source.clip = clip;
                _source.Play();
            }),false);
            await FTask.CompletedTask;
        }
    }
}