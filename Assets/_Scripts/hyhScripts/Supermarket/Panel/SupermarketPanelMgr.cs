using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupermarketPanelMgr<T> : MonoBehaviour where T : class
{
    private static readonly int OpenOrClose = Animator.StringToHash("OpenOrClose");
    private Animator animator;
    
    private static T instance;
    public static T Instance => instance;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        instance = this as T;
    }
    
    public virtual void Show()
    {
        gameObject.SetActive(true);
        animator.SetBool(OpenOrClose,true);
    }

    public virtual void Hide()
    {
        animator.SetBool(OpenOrClose,false);
    }

    public virtual void HideDis()
    {
        gameObject.SetActive(false);
    }
}
