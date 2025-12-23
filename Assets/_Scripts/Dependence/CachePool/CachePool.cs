using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;



#region 使用缓存池缓存一般class接口

public class CachePoolOBJ<T>  where T: CachePoolOBJ<T>,new()
{
    public virtual void Init()
    {

    }
    public virtual void Release()
    {
        CachePool.Instance.Push<T>(this);
    }
}


#endregion



interface ICacheItem
{
}

interface ICacheItem<T>: ICacheItem where T: Component
{
    public T Pop();

    public void Push(T data);
}


class InteralCache<T>: ICacheItem<T> where T: Component
{
    private string resPath;
    private int maxCount;

    private Queue<T> queue = new Queue<T>();
    private List<T> usedList = new List<T>();
    public T Pop()
    {
        T cur;
        if (queue.Count > 0)
        {
            cur = queue.Dequeue();
        }
        else
        {
            if (usedList.Count < maxCount)
            {
                GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(resPath));
                GameObject.DontDestroyOnLoad(go);
                go.name = resPath;
                cur = go.GetComponent<T>();
            }
            else
            {
                cur = usedList[0];
                usedList.Remove(cur);
            }
        }
        usedList.Add(cur);
        cur.gameObject.SetActive(true);
        return cur;
    }

    public void Push(T data)
    {
        data.gameObject.SetActive(false);
        queue.Enqueue(data);
        usedList.Remove(data);
    }

    public void Clear()
    {
        int queueCount = queue.Count;
        for (int i = 0; i < queueCount; i++)
        {
            GameObject.Destroy(queue.Dequeue().gameObject);
        }

        foreach (T behaviour in usedList)
        {
            GameObject.Destroy(behaviour.gameObject);
        }
        queue.Clear();
        usedList.Clear();
    }

    public InteralCache(string resPath)
    {
        this.resPath = resPath;
        GameObject obj = Resources.Load<GameObject>(this.resPath);
        CacheItem item = obj.GetComponent<CacheItem>();
        if (item == null)
        {
            Debug.LogError($"当前对象[{resPath}]没有被缓存池管理");
            return;
        }

        maxCount = item.MaxCount;
    }
}


public class CachePool : BaseMgr<CachePool>
{
    private CachePool() {}

    private Dictionary<string, ICacheItem> interalCacheDic = new Dictionary<string, ICacheItem>();

    private Dictionary<string, Queue<object>> commonClassCacheDic = new Dictionary<string, Queue<object>>();
    public T Pop<T>(string path) where T: Component
    {
        string dicKey = path +"-"+ typeof(T).Name;
        if (!interalCacheDic.ContainsKey(dicKey))
        {
            InteralCache<T> cache = new InteralCache<T>(path);
            interalCacheDic.Add(dicKey,cache);
        }
        return (interalCacheDic[dicKey] as InteralCache<T>).Pop();
    }

    public void Push<T>(T data) where T: Component
    {
        string dicKey = data.gameObject.name + "-"+typeof(T).Name;
        if (interalCacheDic.ContainsKey(dicKey))
        {
            (interalCacheDic[dicKey] as InteralCache<T>).Push(data);
        }
    }

    #region 一般类对象缓存

    public void Push<T>(object data) where T : CachePoolOBJ<T>,new()
    {
        string key = typeof(T).Name;
        if (commonClassCacheDic.ContainsKey(key))
        {
            commonClassCacheDic[key].Enqueue(data);
        }
        else
        {
            Debug.Log($"{key} -- 缓存失败，缓存池中没有对应的类型被记录");
        }
    }

    public T Pop<T>() where T: CachePoolOBJ<T>,new()
    {
        string key = typeof(T).Name;
        if (commonClassCacheDic.ContainsKey(key))
        {
            if (commonClassCacheDic[key].Count > 0)
            {
                return commonClassCacheDic[key].Dequeue() as T;
            }
        }
        else if (!commonClassCacheDic.ContainsKey(key))
        {
            commonClassCacheDic.Add(key,new Queue<object>());
        }

        return new T();
    }


    #endregion

    public void Clear()
    {
        foreach (var cacheItem in interalCacheDic.Values)
        {
            Type cacheType = typeof(CacheItem);
            MethodInfo info = cacheType.GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
            info?.Invoke(cacheItem,null);
        }
        interalCacheDic.Clear();
    }
}
