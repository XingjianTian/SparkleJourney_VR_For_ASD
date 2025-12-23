using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PassageController : MonoBehaviour
{
    [Header("通行控制")]
    [SerializeField] private bool isPassageAllowed = false; // 是否允许通行
    [SerializeField] private string targetTag = "FixedMovementObj"; // 目标物体的标签
    [SerializeField] private float stopLineDistance = 1f; // 停止线距离
    [SerializeField] private float temporaryPassageDuration = 5f; // 临时通行持续时间
    [SerializeField] private float yellowLightDuration = 3f; // 黄灯持续时间

    [Header("视觉效果")]
    [SerializeField] private Material redLightMaterial; // 红灯材质
    [SerializeField] private Material yellowLightMaterial; // 黄灯材质
    [SerializeField] private Material greenLightMaterial; // 绿灯材质
    [SerializeField] private Material defaultLightMaterial; // 默认黑色材质
    [SerializeField] private List<Renderer> redLightRenderers = new List<Renderer>(); // 红灯渲染器列表
    [SerializeField] private List<Renderer> yellowLightRenderers = new List<Renderer>(); // 黄灯渲染器列表
    [SerializeField] private List<Renderer> greenLightRenderers = new List<Renderer>(); // 绿灯渲染器列表

    private TrafficLightController.TrafficLightState currentState = TrafficLightController.TrafficLightState.Red;
    public Dictionary<FixedPointMovement, Coroutine> temporaryPassages;
    private Coroutine yellowLightCoroutine;

    private void Start()
    {
        temporaryPassages = transform.parent.GetComponent<TrafficLightController>().temporaryPassages;

        // 根据isPassageAllowed的初始值设置灯光状态
        currentState = isPassageAllowed ? TrafficLightController.TrafficLightState.Green : TrafficLightController.TrafficLightState.Red;
        UpdateLightDisplay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        FixedPointMovement movement = other.GetComponent<FixedPointMovement>();
        if (movement == null) return;

        Debug.Log($"[{gameObject.name}] 车辆进入触发区域: {other.gameObject.name}");

        // 如果当前允许通行，直接放行
        if (isPassageAllowed)
        {
            Debug.Log($"[{gameObject.name}] 当前允许通行，直接放行: {other.gameObject.name}");
            movement.ResumeMovement();
            return;
        }

        // 检查是否有临时通行权限
        if (temporaryPassages.ContainsKey(movement))
        {
            Debug.Log($"[{gameObject.name}] 有临时通行权限，允许通过: {other.gameObject.name}");
            movement.ResumeMovement();
        }
        else
        {
            Debug.Log($"[{gameObject.name}] 无通行权限，暂停移动: {other.gameObject.name}");
            movement.PauseMovement();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        FixedPointMovement movement = other.GetComponent<FixedPointMovement>();
        if (movement == null) return;

        // 如果当前允许通行或有临时通行权限，允许移动
        if (isPassageAllowed || temporaryPassages.ContainsKey(movement))
        {
            movement.ResumeMovement();
        }
        else
        {
            movement.PauseMovement();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        FixedPointMovement movement = other.GetComponent<FixedPointMovement>();
        if (movement == null) return;

        Debug.Log($"[{gameObject.name}] 车辆离开触发区域: {other.gameObject.name}");

        // 在离开时给予临时通行权限
        if (!temporaryPassages.ContainsKey(movement))
        {
            Debug.Log($"[{gameObject.name}] 给予临时通行权限: {other.gameObject.name}");
            GrantTemporaryPassage(movement);
        }

        movement.ResumeMovement();
    }

    // 给予临时通行权限
    private void GrantTemporaryPassage(FixedPointMovement movement)
    {
        // 如果已经有临时通行权限，先移除
        RemoveTemporaryPassage(movement);

        // 添加新的临时通行权限
        Coroutine coroutine = StartCoroutine(TemporaryPassageCoroutine(movement));
        temporaryPassages[movement] = coroutine;
        Debug.Log($"[{gameObject.name}] 临时通行权限已授予: {movement.gameObject.name}");
    }

    // 移除临时通行权限
    private void RemoveTemporaryPassage(FixedPointMovement movement)
    {
        if (temporaryPassages.ContainsKey(movement))
        {
            if (temporaryPassages[movement] != null)
            {
                StopCoroutine(temporaryPassages[movement]);
            }
            temporaryPassages.Remove(movement);
            Debug.Log($"[{gameObject.name}] 临时通行权限已移除: {movement.gameObject.name}");
        }
    }

    // 临时通行权限协程
    private IEnumerator TemporaryPassageCoroutine(FixedPointMovement movement)
    {
        yield return new WaitForSeconds(temporaryPassageDuration);
        RemoveTemporaryPassage(movement);
        Debug.Log($"[{gameObject.name}] 临时通行权限超时移除: {movement.gameObject.name}");
    }

    // 设置是否允许通行
    public void SetPassageAllowed(bool allowed)
    {
        if (isPassageAllowed == allowed) return; // 如果状态没有改变，直接返回

        if (allowed)
        {
            // 直接切换到绿灯
            isPassageAllowed = true;
            currentState = TrafficLightController.TrafficLightState.Green;
            UpdateLightDisplay();
        }
        else
        {
            // 如果当前是绿灯，先切换到黄灯
            if (currentState == TrafficLightController.TrafficLightState.Green)
            {
                StartYellowLightTransition();
            }
            else
            {
                // 如果当前不是绿灯，直接切换到红灯
                isPassageAllowed = false;
                currentState = TrafficLightController.TrafficLightState.Red;
                UpdateLightDisplay();
            }
        }
        Debug.Log($"[{gameObject.name}] 设置通行状态: {(allowed ? "允许" : "禁止")}");
    }

    // 切换通行状态
    public void TogglePassage()
    {
        SetPassageAllowed(!isPassageAllowed);
    }

    // 开始黄灯过渡
    private void StartYellowLightTransition()
    {
        // 如果已经有黄灯协程在运行，先停止它
        if (yellowLightCoroutine != null)
        {
            StopCoroutine(yellowLightCoroutine);
        }
        yellowLightCoroutine = StartCoroutine(YellowLightTransitionCoroutine());
    }

    // 黄灯过渡协程
    private IEnumerator YellowLightTransitionCoroutine()
    {
        // 切换到黄灯
        currentState = TrafficLightController.TrafficLightState.Yellow;
        UpdateLightDisplay();
        Debug.Log($"[{gameObject.name}] 切换到黄灯");

        // 等待指定时间
        yield return new WaitForSeconds(yellowLightDuration);

        // 切换到红灯
        isPassageAllowed = false;
        currentState = TrafficLightController.TrafficLightState.Red;
        UpdateLightDisplay();
        Debug.Log($"[{gameObject.name}] 切换到红灯");

        yellowLightCoroutine = null;
    }

    // 设置红绿灯状态
    public void SetTrafficLightState(TrafficLightController.TrafficLightState state)
    {
        if (currentState == state) return; // 如果状态没有改变，直接返回

        if (state == TrafficLightController.TrafficLightState.Green)
        {
            // 直接切换到绿灯
            currentState = state;
            isPassageAllowed = true;
            UpdateLightDisplay();
        }
        else if (state == TrafficLightController.TrafficLightState.Red)
        {
            // 如果当前是绿灯，先切换到黄灯
            if (currentState == TrafficLightController.TrafficLightState.Green)
            {
                StartYellowLightTransition();
            }
            else
            {
                // 如果当前不是绿灯，直接切换到红灯
                currentState = state;
                isPassageAllowed = false;
                UpdateLightDisplay();
            }
        }
        else
        {
            // 直接切换到黄灯
            currentState = state;
            UpdateLightDisplay();
        }
        Debug.Log($"[{gameObject.name}] 设置红绿灯状态: {state}");
    }

    // 获取当前通行状态
    public bool IsPassageAllowed()
    {
        return isPassageAllowed;
    }

    // 更新灯光显示
    private void UpdateLightDisplay()
    {
        // 重置所有灯为默认黑色
        foreach (var renderer in redLightRenderers)
        {
            if (renderer != null) renderer.material = defaultLightMaterial;
        }
        foreach (var renderer in yellowLightRenderers)
        {
            if (renderer != null) renderer.material = defaultLightMaterial;
        }
        foreach (var renderer in greenLightRenderers)
        {
            if (renderer != null) renderer.material = defaultLightMaterial;
        }

        // 根据当前状态激活对应的灯
        switch (currentState)
        {
            case TrafficLightController.TrafficLightState.Red:
                foreach (var renderer in redLightRenderers)
                {
                    if (renderer != null) renderer.material = redLightMaterial;
                }
                break;
            case TrafficLightController.TrafficLightState.Yellow:
                foreach (var renderer in yellowLightRenderers)
                {
                    if (renderer != null) renderer.material = yellowLightMaterial;
                }
                break;
            case TrafficLightController.TrafficLightState.Green:
                foreach (var renderer in greenLightRenderers)
                {
                    if (renderer != null) renderer.material = greenLightMaterial;
                }
                break;
        }
    }

    // 在Scene视图中绘制调试信息
    private void OnDrawGizmos()
    {
        // 绘制停止线
        Gizmos.color = Color.red;
        Vector3 stopLinePosition = transform.position + transform.forward * stopLineDistance;
        Gizmos.DrawLine(stopLinePosition + transform.right * 2f, stopLinePosition - transform.right * 2f);

        // 绘制触发区域
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}