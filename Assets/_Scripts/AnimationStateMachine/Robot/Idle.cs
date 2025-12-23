using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Idle : StateMachineBehaviour
{
    public float switch2IdleSleepTime = 30;
    private float currentTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        currentTime += Time.deltaTime;
        if (currentTime >= switch2IdleSleepTime)
        {
            animator.SetTrigger("IdleSleep");
            currentTime = Random.Range(-10, 5);
        }
    }
}
