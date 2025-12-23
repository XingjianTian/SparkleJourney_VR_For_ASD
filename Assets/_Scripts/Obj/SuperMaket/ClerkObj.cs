using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ClerkObj : MonoSingleton<ClerkObj>
{
    public string tone = "5118";
    private Animator animator;

    private AudioSource audioSource;
    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }


    public void DoWelcome()
    {
        transform.DORotateQuaternion(Quaternion.LookRotation(PlayerObj.Instance.transform.position - transform.position), 0.6f);
        animator.SetTrigger("Hellow");
        TimerMgr.Instance.StartTimer(1.6f, (() =>
        {
            transform.DORotateQuaternion(Quaternion.identity, 0.6f);
        }));
    }





    public  void SpeechText(string txt,UnityAction<AudioClip> callback = null)
    {
        Text2SpeechHandle.Instance.Send(txt, callback, true,tone);
    }

}
