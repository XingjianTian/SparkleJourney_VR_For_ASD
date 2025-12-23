using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficLightController : MonoBehaviour
{
    [Header("红绿灯设置")]
    [SerializeField] private PassageController[] passageControllers; // 所有路口的通行控制器
    [SerializeField] private float greenLightDuration = 30f; // 绿灯持续时间
    [SerializeField] private float yellowLightDuration = 3f; // 黄灯持续时间
    [SerializeField] private float redLightDuration = 30f; // 红灯持续时间

    [Header("灯光显示")]
    [SerializeField] private Material redLightMaterial; // 红灯材质
    [SerializeField] private Material yellowLightMaterial; // 黄灯材质
    [SerializeField] private Material greenLightMaterial; // 绿灯材质
    [SerializeField] private Renderer[] trafficLightRenderers; // 红绿灯渲染器

    private int currentPhase = 0; // 当前相位
    private bool isRunning = true; // 是否正在运行

    public Dictionary<FixedPointMovement, Coroutine> temporaryPassages = new Dictionary<FixedPointMovement, Coroutine>();
    // 红绿灯状态枚举
    public enum TrafficLightState
    {
        Red,
        Yellow,
        Green,
    }

    private void Start()
    {
        StartCoroutine(TrafficLightCycle());
        // greenLightDuration = Random.Range(20, 50);
        // redLightDuration = Random.Range(20, 50);
    }

    // 红绿灯循环协程
    private IEnumerator TrafficLightCycle()
    {
        while (isRunning)
        {
            // 绿灯阶段
            SetTrafficLightState(TrafficLightState.Green);
            yield return new WaitForSeconds(greenLightDuration);

            // 黄灯阶段
            SetTrafficLightState(TrafficLightState.Yellow);
            yield return new WaitForSeconds(yellowLightDuration);
            // 红灯阶段
            SetTrafficLightState(TrafficLightState.Red);
            yield return new WaitForSeconds(redLightDuration);
            // 切换到下一个相位
            SwitchPhase();


        }
    }

    // 设置红绿灯状态
    private void SetTrafficLightState(TrafficLightState state)
    {
        switch (state)
        {
            case TrafficLightState.Green:
                // 设置当前相位的路口为绿灯
                for (int i = 0; i < passageControllers.Length; i++)
                {
                    passageControllers[i].SetPassageAllowed(i == currentPhase);
                }
                UpdateLightDisplay(greenLightMaterial);
                break;

            case TrafficLightState.Yellow:
                // 黄灯时保持当前相位的通行状态
                UpdateLightDisplay(yellowLightMaterial);
                break;

            case TrafficLightState.Red:
                // 设置当前相位的路口为红灯
                for (int i = 0; i < passageControllers.Length; i++)
                {
                    passageControllers[i].SetPassageAllowed(false);
                }
                UpdateLightDisplay(redLightMaterial);
                break;
        }
    }

    // 更新灯光显示
    private void UpdateLightDisplay(Material lightMaterial)
    {
        foreach (Renderer renderer in trafficLightRenderers)
        {
            renderer.material = lightMaterial;
        }
    }

    // 切换到下一个相位
    private void SwitchPhase()
    {
        currentPhase = (currentPhase + 1) % passageControllers.Length;
    }

    // 暂停红绿灯系统
    public void PauseSystem()
    {
        isRunning = false;
        StopAllCoroutines();
        // 设置所有路口为红灯
        SetTrafficLightState(TrafficLightState.Red);
    }

    // 恢复红绿灯系统
    public void ResumeSystem()
    {
        isRunning = true;
        StartCoroutine(TrafficLightCycle());
    }

    // 在Scene视图中绘制调试信息
    private void OnDrawGizmos()
    {
        if (passageControllers != null)
        {
            foreach (PassageController controller in passageControllers)
            {
                if (controller != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(controller.transform.position, 1f);
                }
            }
        }
    }
}