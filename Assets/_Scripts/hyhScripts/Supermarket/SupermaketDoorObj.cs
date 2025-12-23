using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupermaketDoorObj : MonoBehaviour
{
    private static readonly int OpenOrClose = Animator.StringToHash("OpenOrClose");
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool(OpenOrClose, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool(OpenOrClose, false);
        }
    }
}
