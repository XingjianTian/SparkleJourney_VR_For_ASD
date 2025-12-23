using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

public class SiliconFlowDeepSeekHandle : BaseMgr<SiliconFlowDeepSeekHandle>
{
    private SiliconFlowDeepSeekHandle(){}
    private static string apiKey = "Bearer sk-vwcpzwyusdtsqovoedfhdqwedncxliquwtdhvjkhdrlnueww";

    private List<ContentItem> context = new List<ContentItem>()
    {
        new ContentItem("user","接下来的回答两句话间请用;;隔开，每句话不超过120 GBK字节，即60个汉字或者字母数字。"),
        new ContentItem("assistant","好的，我明白了;;我会按照你的要求进行回答;;每句话不超过120 GBK字节;;并用;;隔开两句话。")

    };

     public async Task<string> Send(string msg,Queue<TextEvent> resQueue = null)
    {
        //填加上下文
        context.Add(new ContentItem("user",msg));
        //发送请求
        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "https://api.siliconflow.cn/v1/chat/completions");
        req.Headers.Add("Authorization",apiKey);
        object body = new
        {
            model = "Pro/deepseek-ai/DeepSeek-V3",
            messages = context,
            stream = true,
        };
        req.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        StringBuilder fullString = new StringBuilder();

        // TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();
        await NetMgr.Instance.Send(req,(async response =>
        {
            using (Stream resStream = await response.Content.ReadAsStreamAsync())
            {
                StreamReader streamReader = new StreamReader(resStream);
                string line;

                StringBuilder cacheString = new StringBuilder();
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    if (line.StartsWith("data: "))
                    {
                        string jsonStr = line.Substring(6).Trim(new []{'\r','\n'});
                        if (jsonStr == "[DONE]")
                        {
                            if (cacheString.Length > 0)
                            {
                                if (resQueue != null)
                                {
                                    TextEvent textEvent = new TextEvent(cacheString.ToString());
                                    resQueue.Enqueue(textEvent);
                                }

                                cacheString.Clear();
                            }
                            break;
                        }
                        JObject jObject = JObject.Parse(jsonStr);
                        string content =jObject["choices"][0]["delta"]["content"].ToString();
                        if (content.Contains(";;"))
                        {

                            //将完整的一句提交到队列，缓存下一句的内容
                            string[] splitContent = content.Split(";;");
                            cacheString.Append(splitContent[0]);
                            if (resQueue != null)
                            {
                                TextEvent textEvent = new TextEvent(cacheString.ToString());
                                resQueue.Enqueue(textEvent);
                            }
                            cacheString.Clear();
                            cacheString.Append(splitContent[1]);
                        }
                        else
                        {
                            cacheString.Append(content);
                        }
                        fullString.Append(content);
                    }
                }

                // _taskCompletionSource.SetResult(true);
            }
        } ), HttpCompletionOption.ResponseHeadersRead);
        // await _taskCompletionSource.Task;
        Debug.Log(fullString.ToString());
        return fullString.ToString();
    }

    public void ClearContent()
    {

    }



    struct ContentItem
    {
        public string role;
        public string content;


        public ContentItem(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }
}
