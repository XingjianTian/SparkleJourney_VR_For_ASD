using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RobotFollow : MonoBehaviour
{
    [Header("机器人跟随设置")]
    [Tooltip("机器人是否应该一直跟随玩家")]
    public bool shouldFollowPlayer = true;
    [Tooltip("与目标距离的缓冲值")]
    public float distanceBuffer = 2f;
    [Tooltip("是否正在说话")]
    private bool isSpeaking = false;
    [Tooltip("看向玩家的旋转速度")]
    public float lookAtSpeed = 0.5f;
    [Tooltip("是否始终看向玩家")]
    public bool alwaysLookAtPlayer = true;

    private Tween lookAtTween;

    /// <summary>
    /// 设置机器人是否正在说话
    /// </summary>
    public void SetSpeakingState(bool speaking)
    {
        isSpeaking = speaking;
    }

    /// <summary>
    /// 设置是否应该跟随玩家
    /// </summary>
    public void SetShouldFollowPlayer(bool follow)
    {
        shouldFollowPlayer = follow;
    }

    /// <summary>
    /// 设置是否始终看向玩家
    /// </summary>
    public void SetAlwaysLookAtPlayer(bool lookAt)
    {
        alwaysLookAtPlayer = lookAt;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 处理机器人跟随逻辑
        if (shouldFollowPlayer && !isSpeaking)
        {
            // 计算机器人和玩家之间的距离
            float distanceToPlayer = Vector3.Distance(RobotObj.Instance.transform.position, PlayerObj.Instance.transform.position);

            // 只有当距离大于目标距离加上缓冲值时才进行寻路
            if (distanceToPlayer > RobotObj.Instance.navTargetDistance + distanceBuffer)
            {
                RobotObj.Instance.SetDestination(PlayerObj.Instance.transform.position);
            }
            // 当距离小于等于目标距离时停止寻路
            else if (distanceToPlayer <= RobotObj.Instance.navTargetDistance)
            {
                RobotObj.Instance.StopCurrentNavigation();
            }
        }

        // 处理机器人看向玩家逻辑
        if (alwaysLookAtPlayer && !isSpeaking)
        {
            LookAtPlayer();
        }
    }

    /// <summary>
    /// 让机器人看向玩家
    /// </summary>
    private void LookAtPlayer()
    {
        // 如果正在执行旋转Tween，先杀掉它
        if (lookAtTween != null && lookAtTween.IsActive())
        {
            lookAtTween.Kill();
        }

        // 计算需要面向的方向（保持Y轴不变，只在水平面上旋转）
        Vector3 targetPosition = PlayerObj.Instance.transform.position;
        Vector3 robotPosition = RobotObj.Instance.transform.position;
        targetPosition.y = robotPosition.y; // 保持y轴高度相同

        // 计算朝向
        Vector3 direction = targetPosition - robotPosition;
        if (direction != Vector3.zero) // 防止零向量
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 使用DOTween平滑旋转
            lookAtTween = RobotObj.Instance.transform.DORotateQuaternion(targetRotation, lookAtSpeed)
                .SetEase(Ease.OutSine);
        }
    }

    private void OnDisable()
    {
        // 确保在对象禁用时停止所有Tween
        if (lookAtTween != null && lookAtTween.IsActive())
        {
            lookAtTween.Kill();
        }
    }
}
