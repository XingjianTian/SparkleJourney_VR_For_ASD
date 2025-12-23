using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class DeepSeekHandle: BaseMgr<DeepSeekHandle>
{
    private List<Delta> recoardDelta = new List<Delta>(){ new Delta(){Role = "user",Content = "接下来的回答两句话间请用;;隔开，每句话不超过120 GBK字节，即60个汉字或者字母数字。"}, new Delta(){Role = "assistant",Content = "好的，我明白了;;我会按照你的要求进行回答;;每句话不超过120 GBK字节;;并用;;隔开两句话。"}};

    private TaskCompletionSource<bool> _taskCompletionSource;



    public async Task<string> Send(string message,Text txt = null)
    {
        if (_taskCompletionSource != null)
        {
            bool result = await _taskCompletionSource.Task;
            _taskCompletionSource = null;
            if (!result)
            {
                return "";
            }
        }

        _taskCompletionSource = new TaskCompletionSource<bool>();
        recoardDelta.Add(new Delta(){Role = "user",Content = message});



        //发送的json形式的http 请求
        var content = new
        {
            messages = recoardDelta,
            model = "deepseek-chat",
            frequency_penalty = 0,
            max_tokens = 2048,
            presence_penalty = 0,
            response_format = new { type = "text" },
            stop = (string)null,
            stream = true,
            stream_options = (object)null,
            temperature = 1,
            top_p = 1,
            tools = (object)null,
            tool_choice = "none",
            logprobs = false,
            top_logprobs = (object)null
        };
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.deepseek.com/chat/completions");
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", "Bearer sk-81b0ef86964240f292697d210d29710f");
        StringContent body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        request.Content = body;
        StringBuilder fullContent = new StringBuilder();
        await NetMgr.Instance.Send(request, async (response) =>
        {
            //预先添加相应内容，防止顺序错乱
            recoardDelta.Add(new Delta());
            int responsePointer = recoardDelta.Count - 1;
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            {
                StreamReader reader = new StreamReader(stream);
                string line;
                string cache = "";
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line.StartsWith("data:"))
                    {
                        string jsonStr = line.Substring("data:".Length).Trim();
                        if (jsonStr == "[DONE]")
                        {
                            recoardDelta[responsePointer].Role = "assistant";
                            recoardDelta[responsePointer].Content = fullContent.ToString();
                            if (cache != "")
                            {
                                // DeepseekAsistant.Instance.textQueue.Enqueue(cache);
                            }
                            _taskCompletionSource.SetResult(true);
                            break;
                        }

                        ChatCompletionChunk reply = JsonConvert.DeserializeObject<ChatCompletionChunk>(jsonStr);
                        string replyContent = reply.Choices[0].Delta.Content;
                        if (replyContent.Contains(";;"))
                        {
                            string[] temp = replyContent.Split(";;");
                            cache += temp[0];
                            // DeepseekAsistant.Instance.textQueue.Enqueue(cache);
                            cache = "";
                            cache += temp[1];
                        }
                        else
                        {
                            cache += replyContent;
                        }

                        if (txt != null)
                        {
                            txt.text += replyContent;
                        }
                        fullContent.Append(replyContent);
                    }

                }
            }
            response.Dispose();
        },HttpCompletionOption.ResponseHeadersRead);
        return fullContent.ToString();
    }

    public void ClearContent()
    {
        recoardDelta.Clear();
    }

    private DeepSeekHandle()
    {

    }
}
class ChatCompletionChunk
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("system_fingerprint")]
    public string SystemFingerprint { get; set; }

    [JsonProperty("choices")]
    public List<Choice> Choices { get; set; }

    [JsonProperty("usage")]
    public Usage Usage { get; set; }
}

 class Choice
{
    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("delta")]
    public Delta Delta { get; set; }

    [JsonProperty("logprobs")]
    public object Logprobs { get; set; }

    [JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
}

 class Delta
{
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}

 class Usage
{
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }

    [JsonProperty("prompt_tokens_details")]
    public PromptTokensDetails PromptTokensDetails { get; set; }

    [JsonProperty("prompt_cache_hit_tokens")]
    public int PromptCacheHitTokens { get; set; }

    [JsonProperty("prompt_cache_miss_tokens")]
    public int PromptCacheMissTokens { get; set; }
}

 class PromptTokensDetails
{
    [JsonProperty("cached_tokens")]
    public int CachedTokens { get; set; }
}

