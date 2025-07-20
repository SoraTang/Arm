using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class LeverInitializer : MonoBehaviour
{
    [Header("杠杆设置")]
    public XRLever[] levers;  // 所有需要初始化的杠杆

    [Header("初始化设置")]
    public bool defaultState = false;  // 默认状态（false = 非激活，true = 激活）

    void Start()
    {
        InitializeLevers();
    }

    void InitializeLevers()
    {
        if (levers == null || levers.Length == 0) return;

        foreach (var lever in levers)
        {
            if (lever != null)
            {
                // 设置杠杆的初始状态
                lever.value = defaultState;

                // 确保杠杆锁定到值位置
                lever.lockToValue = true;

                Debug.Log($"杠杆 {lever.name} 已设置为 {(defaultState ? "激活" : "非激活")} 状态");
            }
        }
    }

    // 手动设置所有杠杆状态
    public void SetAllLeversState(bool state)
    {
        if (levers == null || levers.Length == 0) return;

        foreach (var lever in levers)
        {
            if (lever != null)
            {
                lever.value = state;
            }
        }
    }

    // 设置特定杠杆状态
    public void SetLeverState(XRLever lever, bool state)
    {
        if (lever != null)
        {
            lever.value = state;
        }
    }
}