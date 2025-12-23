using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoMonoSingleton<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                GameObject.DontDestroyOnLoad(go);
                _instance = go.AddComponent<T>();
            }

            return _instance;
        }
    }
}
