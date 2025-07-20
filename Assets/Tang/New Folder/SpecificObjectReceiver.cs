using UnityEngine;

public class SpecificObjectReceiver : MonoBehaviour
{
    [Header("检测的特定物体")]
    public GameObject targetObject;

    [Header("触发后要激活的物体列表")]
    public GameObject[] objectsToActivate;

    [Header("触发后要关闭的物体列表")]
    public GameObject[] objectsToDeactivate;

    [Header("是否只触发一次")]
    public bool triggerOnlyOnce = true;

    [Header("是否已触发")]
    private bool hasTriggered = false;

    [Header("触发后通知门控制器（可选）")]
    public GateTriggerController controller;
    public int zoneID = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnlyOnce) return;

        if (other.gameObject == targetObject)
        {
            hasTriggered = true;

            // 关闭物体
            foreach (var go in objectsToDeactivate)
                if (go != null) go.SetActive(false);

            // 激活物体
            foreach (var go in objectsToActivate)
                if (go != null) go.SetActive(true);

            // 通知控制器
            if (controller != null)
                controller.NotifyZoneTriggered(zoneID);

            Debug.Log($"SpecificObjectReceiver 已触发，区域 {zoneID} 完成。");
        }
    }
}
