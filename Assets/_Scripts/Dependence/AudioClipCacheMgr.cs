using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Exception = System.Exception;

public class AudioClipCacheMgr : BaseMgr<AudioClipCacheMgr>
{
    public static string BASE_PATH = Path.Combine(Application.persistentDataPath, "AuidoCache");

    public static ConcurrentDictionary<string, AudioClip>
        audioClipCacheDic = new ConcurrentDictionary<string, AudioClip>();

    public async Task<bool> StoreAudioClip(string content, AudioClip clip)
    {
        string key = FilterSpecialCharacters(content);
        string fullPath = Path.Combine(BASE_PATH, key);

        try
        {
            if (key.Length < 30)
            {
                await File.WriteAllBytesAsync(fullPath, WavUtility.ConvertAudioClipToWav(clip));
                Debug.Log($"存储音频{fullPath} 成功，路径：{fullPath}");
            }
            audioClipCacheDic.TryAdd(key, clip);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public AudioClip GetAudioClip(string content)
    {
        string key = FilterSpecialCharacters(content);
        if (audioClipCacheDic.ContainsKey(key))
        {
            return audioClipCacheDic[key];
        }

        return null;
    }

    private AudioClipCacheMgr()
    {
        InitCache();
    }

    private void InitCache()
    {
        if (!Directory.Exists(BASE_PATH))
        {
            Directory.CreateDirectory(BASE_PATH);
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(BASE_PATH);
        FileInfo[] fileinfoColletion = directoryInfo.GetFiles();
        for (int i = fileinfoColletion.Length - 1; i >=0; i--)
        {
            FileInfo fileInfo = fileinfoColletion[i];
            TimeSpan gapTime = DateTime.UtcNow - fileInfo.CreationTimeUtc;
            if (gapTime > new TimeSpan(7,0,0))
            {
                fileInfo.Delete();
            }
            else
            {
                audioClipCacheDic.TryAdd(fileInfo.Name, WavUtility.ToAudioClip(File.ReadAllBytes(fileInfo.FullName)));
            }
        }
        Debug.Log($"初始化音频缓存成功，缓存音频个数：{audioClipCacheDic.Count}");
    }
    private string FilterSpecialCharacters(string input, bool keepNumbers = true)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // 定义正则表达式模式
        string pattern;
        if (keepNumbers)
        {
            // 匹配中文、英文和数字
            pattern = @"[^\u4e00-\u9fa5a-zA-Z0-9]";
        }
        else
        {
            // 匹配中文和英文
            pattern = @"[^\u4e00-\u9fa5a-zA-Z]";
        }

        // 使用正则表达式替换非匹配字符
        return Regex.Replace(input, pattern, "");
    }
}
