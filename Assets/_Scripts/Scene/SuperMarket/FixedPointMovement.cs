using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPointMovement : MonoBehaviour
{
    [Header("寻路设置")]
    [SerializeField] private List<Transform> waypoints = new List<Transform>(); // 路径点列表
    [SerializeField] private float moveSpeed = 5f; // 移动速度
    [SerializeField] private float rotationSpeed = 5f; // 旋转速度
    [SerializeField] private float waypointThreshold = 0.1f; // 到达路径点的阈值
    [SerializeField] private float preTurnDistance = 2f; // 提前转向的距离
    [SerializeField] private float turnStateDistance = 1f; // 切换转弯状态的距离
    [SerializeField] private float earlyTurnWarningDistance = 5f; // 提前显示转弯状态的距离

    [Header("安全距离设置")]
    [SerializeField] private float safetyDistance = 2f; // 安全距离
    [SerializeField] private float raycastDistance = 5f; // 射线检测距离
    [SerializeField] private LayerMask detectionLayerMask; // 检测层级

    private int currentWaypointIndex = 0; // 当前目标路径点索引
    private bool isMoving = true; // 是否正在移动
    private bool isPaused = false; // 是否暂停移动
    private bool isWaitingForClearPath = false; // 是否在等待路径清除
    public PathAction nextAction = PathAction.Straight; // 下一步动作

    // 路径动作枚举
    public enum PathAction
    {
        Straight,   // 直行
        TurnLeft,   // 左转
        TurnRight,  // 右转
        UTurn      // 掉头
    }

    // Start is called before the first frame update
    void Start()
    {
        earlyTurnWarningDistance = 20;
        // 销毁所有MeshCollider
        MeshCollider[] meshColliders = GetComponentsInChildren<MeshCollider>();
        foreach (MeshCollider collider in meshColliders)
        {
            Destroy(collider);
        }

        // 添加并设置Rigidbody
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll; // 锁定所有位置和旋转
        rb.useGravity = false; // 不使用重力

        if (waypoints.Count == 0)
        {
            Debug.LogWarning("没有设置路径点！请在Inspector中添加路径点。");
            isMoving = false;
        }

        gameObject.tag = "FixedMovementObj";
        gameObject.layer = LayerMask.NameToLayer("FixedMovementObj"); // 直接使用层的索引值
        detectionLayerMask = 1 << LayerMask.NameToLayer("FixedMovementObj"); // LayerMask需要位掩码
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving || waypoints.Count == 0 || isPaused) return;

        // 获取当前目标路径点
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // 计算移动方向
        Vector3 direction = targetWaypoint.position - transform.position;
        direction.y = 0; // 保持Y轴不变

        // 检查是否需要提前转向
        CheckNextAction();

        // 旋转面向目标
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 检查前方是否有其他物体
        if (!CheckPathClear())
        {
            isWaitingForClearPath = true;
            return;
        }
        else if (isWaitingForClearPath)
        {
            isWaitingForClearPath = false;
        }

        // 移动向目标
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        // 检查是否到达当前路径点
        if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointThreshold)
        {
            // 移动到下一个路径点
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
    }

    // 检查下一步动作
    private void CheckNextAction()
    {
        if (waypoints.Count < 2) return;

        // 获取当前点和下一个点
        Transform currentPoint = waypoints[currentWaypointIndex];
        Transform nextPoint = waypoints[(currentWaypointIndex + 1) % waypoints.Count];

        // 计算到当前目标点的距离
        float distanceToCurrent = Vector3.Distance(transform.position, currentPoint.position);

        // 计算当前段和下一段的方向
        Vector3 currentDirection = (currentPoint.position - transform.position).normalized;
        Vector3 nextDirection = (nextPoint.position - currentPoint.position).normalized;

        // 计算转向角度
        float angle = Vector3.SignedAngle(currentDirection, nextDirection, Vector3.up);

        // 根据角度判断动作
        PathAction nextTurnAction = PathAction.Straight;
        if (angle > 45f)
        {
            nextTurnAction = PathAction.TurnRight;
        }
        else if (angle < -45f)
        {
            nextTurnAction = PathAction.TurnLeft;
        }
        else if (Mathf.Abs(angle) > 135f)
        {
            nextTurnAction = PathAction.UTurn;
        }

        // 如果距离足够近，显示转弯状态
        if (distanceToCurrent < earlyTurnWarningDistance && nextTurnAction != PathAction.Straight)
        {
            nextAction = nextTurnAction;
        }
        else if (nextAction != PathAction.Straight)
        {
            // 如果当前是转弯状态，检查是否已经离开转弯点足够远
            float distanceToNext = Vector3.Distance(transform.position, nextPoint.position);
            if (distanceToNext > earlyTurnWarningDistance)
            {
                nextAction = PathAction.Straight;
            }
        }
        else
        {
            // 如果当前是直行状态，检查是否需要开始转弯
            Vector3 objectForward = transform.forward;
            Vector3 targetDirection = (currentPoint.position - transform.position).normalized;
            float currentAngle = Vector3.SignedAngle(objectForward, targetDirection, Vector3.up);

            // 如果距离大于切换距离或角度已经很小，设置为直行
            if (distanceToCurrent > turnStateDistance || Mathf.Abs(currentAngle) < 5f)
            {
                nextAction = PathAction.Straight;
            }
        }
    }

    // 检查前方路径是否畅通
    private bool CheckPathClear()
    {
        // 计算射线起点（向上偏移）
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;

        // 发射射线检测前方物体
        RaycastHit[] hits = Physics.RaycastAll(rayStart, transform.forward, raycastDistance, detectionLayerMask);

        foreach (RaycastHit hit in hits)
        {
            // 忽略自身
            if (hit.collider.gameObject == gameObject)
                continue;

            // 检查是否是目标物体
            if (hit.collider.CompareTag("FixedMovementObj"))
            {
                // 计算与前方物体的距离
                float distance = Vector3.Distance(transform.position, hit.point);
                if (distance < safetyDistance)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // 暂停移动
    public void PauseMovement()
    {
        isPaused = true;
    }

    // 恢复移动
    public void ResumeMovement()
    {
        isPaused = false;
    }

    // 在Scene视图中绘制检测范围和路径信息
    private void OnDrawGizmos()
    {
        // 计算射线起点（向上偏移）
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;

        // 绘制射线
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(rayStart, transform.forward * raycastDistance);

        // 绘制安全距离范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rayStart + transform.forward * safetyDistance, 0.2f);

        // 绘制路径和动作提示
        if (waypoints != null && waypoints.Count > 0)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] == null) continue;

                // 绘制路径点
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);

                // 绘制路径线
                if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }

            // 绘制当前动作提示
            Gizmos.color = Color.yellow;
            Vector3 actionPos = transform.position + Vector3.up * 2f;
            switch (nextAction)
            {
                case PathAction.TurnLeft:
                    Gizmos.DrawLine(actionPos, actionPos + Vector3.left);
                    break;
                case PathAction.TurnRight:
                    Gizmos.DrawLine(actionPos, actionPos + Vector3.right);
                    break;
                case PathAction.UTurn:
                    Gizmos.DrawLine(actionPos, actionPos + Vector3.forward);
                    Gizmos.DrawLine(actionPos + Vector3.forward, actionPos + Vector3.forward + Vector3.left);
                    break;
                case PathAction.Straight:
                    Gizmos.DrawLine(actionPos, actionPos + Vector3.forward);
                    break;
            }
        }
    }
}
