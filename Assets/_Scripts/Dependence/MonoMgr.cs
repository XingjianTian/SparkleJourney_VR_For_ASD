using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr : AutoMonoSingleton<MonoMgr>
{
    private event UnityAction UpdateEvnet;
    private event UnityAction FixedUpdateEvent;
    private event UnityAction LateUpdateEvnet;
    private event UnityAction ApplicationQuitEvent;
    private event UnityAction ApplicationQuitEventHighPriority;

    private void OnApplicationQuit()
    {
        ApplicationQuitEventHighPriority?.Invoke();
        ApplicationQuitEvent?.Invoke();
    }

    public void AddApplicationQuitEvent(UnityAction action)
    {
        ApplicationQuitEvent += action;
    }

    public void AddApplicationQuitEventHighPriority(UnityAction action)
    {
        ApplicationQuitEventHighPriority += action;
    }
    public void RemoveApplicationQuitEvent(UnityAction action)
    {
        ApplicationQuitEvent -= action;
    }
    public void RemoveApplicationQuitEventHighPriority(UnityAction action)
    {
        ApplicationQuitEventHighPriority -= action;
    }


    public void AddUpdateEvent(UnityAction action)
    {
        UpdateEvnet += action;
    }

    public void AddFixedUpdateEvent(UnityAction action)
    {
        FixedUpdateEvent += action;
    }

    public void AddLateUpdateEvent(UnityAction action)
    {
        LateUpdateEvnet += action;
    }

    public void RemoveUpdateEvent(UnityAction action)
    {
        UpdateEvnet -= action;
    }

    public void RemoveFixedUpdateEvent(UnityAction action)
    {
        FixedUpdateEvent -= action;
    }

    public void RemoveLateUpdateEvent(UnityAction action)
    {
        LateUpdateEvnet -= action;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEvnet?.Invoke();
    }

    private void FixedUpdate()
    {
        FixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        LateUpdateEvnet?.Invoke();
    }
}
