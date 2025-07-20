using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class AdvancedLeverController : MonoBehaviour
{
    [Header("杠杆设置")]
    public XRLever lever1;
    public XRLever lever2;

    [Header("移动物体设置")]
    public Transform targetObject;
    public float moveSpeed = 2f;
    public float maxHeight = 5f;
    public float minHeight = 0f;

    [Header("移动模式")]
    public bool autoReturn = true;    // 是否自动返回
    public bool smoothMovement = true; // 是否平滑移动

    private bool isMoving = false;
    private bool isMovingUp = false;
    private Vector3 originalPosition;
    private float targetHeight;

    void Start()
    {
        if (targetObject != null)
        {
            originalPosition = targetObject.position;
            targetHeight = minHeight;
        }

        // 监听杠杆状态变化
        if (lever1 != null)
        {
            lever1.onLeverActivate.AddListener(OnLeverStateChanged);
            lever1.onLeverDeactivate.AddListener(OnLeverStateChanged);
        }

        if (lever2 != null)
        {
            lever2.onLeverActivate.AddListener(OnLeverStateChanged);
            lever2.onLeverDeactivate.AddListener(OnLeverStateChanged);
        }
    }

    void OnLeverStateChanged()
    {
        bool bothActive = (lever1 != null && lever1.value) && (lever2 != null && lever2.value);

        if (bothActive)
        {
            // 两个杠杆都激活，向上移动
            isMoving = true;
            isMovingUp = true;
            targetHeight = maxHeight;
            Debug.Log("两个杠杆都激活，开始向上移动");
        }
        else
        {
            // 杠杆停用
            if (autoReturn)
            {
                // 自动返回
                isMoving = true;
                isMovingUp = false;
                targetHeight = minHeight;
                Debug.Log("杠杆停用，开始向下移动");
            }
            else
            {
                // 停止移动
                isMoving = false;
                Debug.Log("杠杆停用，停止移动");
            }
        }
    }

    void Update()
    {
        if (targetObject == null || !isMoving) return;

        Vector3 currentPos = targetObject.position;
        float currentHeight = currentPos.y;

        if (smoothMovement)
        {
            // 平滑移动
            float newY = Mathf.MoveTowards(currentHeight, targetHeight, moveSpeed * Time.deltaTime);
            targetObject.position = new Vector3(currentPos.x, newY, currentPos.z);

            // 检查是否到达目标高度
            if (Mathf.Approximately(newY, targetHeight))
            {
                isMoving = false;
                Debug.Log(isMovingUp ? "到达最高点" : "到达最低点");
            }
        }
        else
        {
            // 直接设置位置
            targetObject.position = new Vector3(currentPos.x, targetHeight, currentPos.z);
            isMoving = false;
        }
    }

    // 手动控制移动
    public void MoveToHeight(float height)
    {
        if (targetObject != null)
        {
            targetHeight = Mathf.Clamp(height, minHeight, maxHeight);
            isMoving = true;
        }
    }

    // 重置位置
    public void ResetPosition()
    {
        if (targetObject != null)
        {
            targetObject.position = originalPosition;
            targetHeight = minHeight;
            isMoving = false;
        }
    }

    void OnDestroy()
    {
        // 清理事件监听
        if (lever1 != null)
        {
            lever1.onLeverActivate.RemoveListener(OnLeverStateChanged);
            lever1.onLeverDeactivate.RemoveListener(OnLeverStateChanged);
        }

        if (lever2 != null)
        {
            lever2.onLeverActivate.RemoveListener(OnLeverStateChanged);
            lever2.onLeverDeactivate.RemoveListener(OnLeverStateChanged);
        }
    }
}