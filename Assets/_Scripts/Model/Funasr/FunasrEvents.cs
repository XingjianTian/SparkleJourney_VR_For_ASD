using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// FunASR事件处理类
/// </summary>
public static class FunasrEvents
{
    // 识别结果事件
    public static event Action<string> OnRecognitionResult;

    // 临时结果事件（仅适用于2pass模式）
    public static event Action<string> OnInterimResult;

    // 识别结束事件
    public static event Action<bool> OnRecognitionEnd;

    // 连接状态事件
    public static event Action<bool, string> OnConnectionStatus;

    // 服务端错误事件
    public static event Action<string> OnServerError;

    // 原始消息事件
    public static event Action<string> OnRawMessage;

    public static void TriggerRawMessage(string message)
    {
        if (OnRawMessage != null)
        {
            OnRawMessage.Invoke(message);
        }
    }

    // 内部触发方法
    internal static void TriggerRecognitionResult(string result)
    {
        if (OnRecognitionResult != null)
        {
            OnRecognitionResult.Invoke(result);
        }
    }

    internal static void TriggerInterimResult(string interimResult)
    {
        if (OnInterimResult != null)
        {
            OnInterimResult.Invoke(interimResult);
        }
    }

    internal static void TriggerRecognitionEnd(bool success)
    {
        if (OnRecognitionEnd != null)
        {
            OnRecognitionEnd.Invoke(success);
        }
    }

    internal static void TriggerConnectionStatus(bool connected, string message)
    {
        if (OnConnectionStatus != null)
        {
            OnConnectionStatus.Invoke(connected, message);
        }
    }

    internal static void TriggerServerError(string errorMessage)
    {
        if (OnServerError != null)
        {
            OnServerError.Invoke(errorMessage);
        }
    }
}