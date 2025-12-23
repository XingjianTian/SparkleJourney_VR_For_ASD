using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BaseMgr<T> where T: class
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                ConstructorInfo constructorInfo =
                    typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes,
                        null);
                _instance = constructorInfo?.Invoke(null) as T;
            }

            return _instance;
        }
    }
}
