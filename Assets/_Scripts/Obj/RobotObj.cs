using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class RobotObj : MonoSingleton<RobotObj>
{
    public float navTargetDistance = 0.1f;

    private NavMeshAgent m_agent;
    private NavMeshAgent navMeshAgent
    {
        get
        {
            if (m_agent == null)
            {
                m_agent = GetComponent<NavMeshAgent>();
            }

            return m_agent;
        }
    }

    private Animator m_animator;

    public Animator animator
    {
        get
        {
            if (m_animator == null)
            {
                m_animator = GetComponent<Animator>();
            }

            return m_animator;
        }
    }

    // 寻路相关变量
    private Vector3 curDes;
    private UnityAction navCallback;
    private Coroutine navCoroutine;
    private bool isNavigating = false;

    /// <summary>
    /// 设置寻路目标位置
    /// </summary>
    /// <param name="des">目标位置</param>
    /// <param name="callback">到达目标后的回调函数</param>
    /// <returns>是否成功开始寻路</returns>
    public bool SetDestination(Vector3 des, UnityAction callback = null)
    {
        // 取消之前的寻路任务
        StopCurrentNavigation();

        // 检查目标点是否可到达
        NavMeshPath path = new NavMeshPath();
        if (!NavMesh.CalculatePath(transform.position, des, navMeshAgent.areaMask, path) || path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogWarning("目标点不可到达！");
            return false;
        }

        // 设置新的寻路目标
        curDes = des;
        navCallback = callback;
        navMeshAgent.enabled = true;

        // 启动寻路
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(des);

        // 设置行走动画
        animator.SetBool("IsWalk", true);
        isNavigating = true;

        // 启动协程监控寻路状态
        navCoroutine = StartCoroutine(MonitorNavigation());

        return true;
    }

    /// <summary>
    /// 停止当前寻路
    /// </summary>
    public void StopCurrentNavigation()
    {
        if (navCoroutine != null)
        {
            StopCoroutine(navCoroutine);
            navCoroutine = null;
        }

        if (isNavigating)
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("IsWalk", false);
            isNavigating = false;
        }
    }

    /// <summary>
    /// 监控寻路状态的协程
    /// </summary>
    private IEnumerator MonitorNavigation()
    {
        while (isNavigating)
        {
            // 检查是否到达目标点附近
            if (Vector3.Distance(transform.position, curDes) < navTargetDistance)
            {
                CompleteNavigation();
                yield break;
            }

            // 检查是否停止移动但未到达目标（可能被卡住）
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude < 0.1f)
                {
                    CompleteNavigation();
                    yield break;
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// 完成寻路
    /// </summary>
    private void CompleteNavigation()
    {
        isNavigating = false;
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
        animator.SetBool("IsWalk", false);

        UnityAction tempCallback = navCallback;
        navCallback = null;

        if (tempCallback != null)
        {
            tempCallback.Invoke();
        }
    }

    public void SpeechText(string content, UnityAction callback = null)
    {
        DeepseekAsistant.Instance.SpeechByString(content, callback);
    }

    public void SpeechByAudioClip(AudioClip clip, UnityAction callback = null)
    {
        DeepseekAsistant.Instance.SpeechBuildIn(clip, callback);
    }

    public void SpeechInResorucesAudio(string path, UnityAction callback = null)
    {
        DeepseekAsistant.Instance.SpeechBuildIn(path, callback);
    }

    void Start()
    {
    }

    void OnDisable()
    {
    }

    public void LookAtPlayer()
    {

    }
}
