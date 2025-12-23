using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Video;

public class SayHellow : StateMachineBehaviour
{
    private static UnityAction endCallback;

    public static void AddLisntener(UnityAction action)
    {
        endCallback += action;
    }

    private VideoClip videoClip;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);


    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        if (endCallback != null)
        {
            endCallback?.Invoke();
            endCallback = null;
        }
    }
}
