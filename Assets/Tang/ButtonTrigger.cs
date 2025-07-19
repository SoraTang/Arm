using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    [Tooltip("是否只触发一次")]
    public bool triggerOnlyOnce = true;

    private bool hasBeenPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是按钮
        if (other.CompareTag("Button"))
        {
            // 如果设置了仅触发一次，且已触发过，则不再触发
            if (triggerOnlyOnce && hasBeenPressed)
                return;

            hasBeenPressed = true;
            Debug.Log("按钮已按下");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 如果允许重复触发，则在按钮离开后重置状态
        if (!triggerOnlyOnce && other.CompareTag("Button"))
        {
            hasBeenPressed = false;
        }
    }
}
