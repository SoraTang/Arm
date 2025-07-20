using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class AutoResetLever : MonoBehaviour
{
    [Header("杠杆设置")]
    public XRLever lever;

    [Header("重置设置")]
    public bool autoReset = true;     // 是否自动重置

    void Start()
    {
        if (lever == null)
        {
            lever = GetComponent<XRLever>();
        }

        if (lever != null)
        {
            // 监听抓取开始事件
            lever.selectEntered.AddListener(OnGrabStart);
            lever.selectExited.AddListener(OnGrabEnd);

            // 确保杠杆默认非激活
            lever.value = false;
        }
    }

    void OnGrabStart(SelectEnterEventArgs args)
    {
        Debug.Log("杠杆被抓取");
    }

    void OnGrabEnd(SelectExitEventArgs args)
    {
        // 立即将杠杆设为非激活状态
        if (autoReset)
        {
            lever.value = false;
            Debug.Log("松开杠杆，立即设为非激活状态");
        }
    }

    // 手动重置杠杆
    public void ManualReset()
    {
        if (lever != null)
        {
            lever.value = false;
            Debug.Log("手动重置杠杆");
        }
    }



    void OnDestroy()
    {
        if (lever != null)
        {
            lever.selectEntered.RemoveListener(OnGrabStart);
            lever.selectExited.RemoveListener(OnGrabEnd);
        }
    }
}