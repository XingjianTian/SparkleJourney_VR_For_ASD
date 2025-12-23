using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Text2SpeechHandle :BaseMgr<Text2SpeechHandle>
{
    private Text2SpeechHandle(){}
    private string accessToken = "";
    private Task<string> getAccessTokenTask;


    public async Task<byte[]> Send(string msg,UnityAction<AudioClip> callback = null,bool isStore = true,string tone = "103")
    {
        if (msg == "")
        {
            Debug.LogWarning("无法将空字符转为语音");
            return null;
        }

        AudioClip cacheClip= AudioClipCacheMgr.Instance.GetAudioClip(msg);
        if (cacheClip != null)
        {
            callback?.Invoke(cacheClip);
            return WavUtility.ConvertAudioClipToWav(cacheClip);
        }
        if (accessToken == "")
        {
            if (getAccessTokenTask == null)
            {
                getAccessTokenTask = GetToken("JT5VbFp92UzEqVRVbCE2AGeD","iGYwnSAPUdiY4x0FxSjeGU82iZcrAh9r");
            }
            await getAccessTokenTask;
            Debug.Log("token获取成功" + accessToken);
        }

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://tsn.baidu.com/text2audio");
        FormUrlEncodedContent formContent = new FormUrlEncodedContent(new []
        {
            new KeyValuePair<string, string>("tex",msg),
            new KeyValuePair<string, string>("tok",accessToken),
            new KeyValuePair<string, string>("cuid","11"),
            new KeyValuePair<string, string>("ctp","1"),
            new KeyValuePair<string, string>("lan","zh"),
            new KeyValuePair<string, string>("per",tone),
            new KeyValuePair<string, string>("aue","6")
        });

        request.Content = formContent;
        byte[] clip = null;
        await NetMgr.Instance.Send(request, (async response =>
        {
            if (response.Content.Headers.ContentType.MediaType != "audio/wav")
            {
                Debug.LogError($"文本转语音失败");
                return;
            }
            // AudioSource audioSource = GameObject.Find("MusicMgr").AddComponent<AudioSource>();
            // audioSource.clip = WavUtility.ToAudioClip(await response.Content.ReadAsByteArrayAsync());
            // audioSource.Play();
            clip = await response.Content.ReadAsByteArrayAsync();
            callback?.Invoke(WavUtility.ToAudioClip(clip));
            response.Dispose();
        }));
        if (isStore)
        {
            AudioClipCacheMgr.Instance.StoreAudioClip(msg, WavUtility.ToAudioClip(clip));
        }
        return clip;
    }
    private async Task<string> GetToken(string appid,string skey)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id={appid}&client_secret={skey}");
        request.Headers.Add("Accept","application/json");
        accessToken = "";
        await NetMgr.Instance.Send(request, (async response =>
        {
            string data = await response.Content.ReadAsStringAsync();
            JObject jsonObj = JObject.Parse(data);
            accessToken = jsonObj["access_token"].ToString();
        }));
        return accessToken;
    }


}
