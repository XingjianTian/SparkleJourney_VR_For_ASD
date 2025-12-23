using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Speech2TextHandle: BaseMgr<Speech2TextHandle>
{
    private Speech2TextHandle(){}
    private string accessToken = "";
    // private TaskCompletionSource<string> _taskCompletionSource;
    private Task<string> getAccessTokenTask;

    public async Task<string> Send(AudioClip clip)
    {
        if (accessToken == "")
        {
            if (getAccessTokenTask == null)
            {
                getAccessTokenTask = GetToken();
            }
            await getAccessTokenTask;
        }

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"https://vop.baidu.com/pro_api?dev_pid=80001&cuid=111&token={accessToken}");
        Save(Application.persistentDataPath+"/a.wav",clip);
        // request.Content = new StreamContent(stream);
        request.Content = new StreamContent(new MemoryStream(File.ReadAllBytes(Application.persistentDataPath+"/a.wav")));
        string content = "";
        request.Content.Headers.Add("Content-Type","audio/wav;rate=16000");

        await NetMgr.Instance.Send(request, async (response) =>
        {
            Debug.Log(await response.Content.ReadAsStringAsync());
            JObject jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            content = jObject["result"].ToString().Trim(new []{'[',']',' ','"','\n','\r'});
        });
        return content;
    }

    private async Task<string> GetToken()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=yUN8pLcH7IhiO5qgnqcbUsXT&client_secret=Bxi75cSZdgi2od7tiNVYNyVmNr43gBH7");
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


    public bool Save(string filepath, AudioClip clip)
    {
        if (!filepath.ToLower().EndsWith(".wav"))
        {
            filepath += ".wav";
        }

        Directory.CreateDirectory(Path.GetDirectoryName(filepath));

        using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(fileStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + clip.samples * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)clip.channels);
                writer.Write(clip.frequency);
                writer.Write(clip.frequency * clip.channels * 2);
                writer.Write((ushort)(clip.channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(clip.samples * 2);

                float[] samples = new float[clip.samples];
                clip.GetData(samples, 0);

                for (int i = 0; i < samples.Length; i++)
                {
                    short sample = (short)(samples[i] * short.MaxValue);
                    writer.Write(sample);
                }
                fileStream.Flush();
            }
        }

        Debug.Log("Audio saved to: " + filepath);
        return true;
    }
}
