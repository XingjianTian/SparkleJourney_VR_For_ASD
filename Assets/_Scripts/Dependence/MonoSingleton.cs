using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MonoSingleton<T> : MonoBehaviour where T: MonoBehaviour
{

    private static T _instance;
    public static T Instance => _instance;
    protected virtual void Awake()
    {
        _instance = this as T;
    }
}
