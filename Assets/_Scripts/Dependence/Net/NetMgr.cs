using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class NetMgr: BaseMgr<NetMgr>
{
    private NetMgr()
    {
        _client = new HttpClient();
    }
    private HttpClient _client;


    public async Task Send(HttpRequestMessage req,UnityAction<HttpResponseMessage> callback,HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead,UnityAction failCallback = null)
    {
        HttpResponseMessage response = await _client.SendAsync(req,httpCompletionOption);
        if (response.IsSuccessStatusCode)
        {
            callback?.Invoke(response);
        }
        else
        {
            failCallback?.Invoke();
            Debug.LogError($"{response.StatusCode}");
        }

    }
}
