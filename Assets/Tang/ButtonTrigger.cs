using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public enum ButtonActionMode
    {
        MoveObject,
        HideObject
    }

    [Header("通用设置")]
    [Tooltip("是否只触发一次")]
    public bool triggerOnlyOnce = true;

    [Tooltip("选择按钮触发模式")]
    public ButtonActionMode actionMode = ButtonActionMode.MoveObject;

    private bool hasBeenPressed = false;

    [Header("移动物体设置（用于 MoveObject 模式）")]
    public Transform targetObjectToMove;
    public float moveDistance = 0.2f;
    public float moveSpeed = 1f;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    [Header("隐藏物体设置（用于 HideObject 模式）")]
    public GameObject objectToHide;

    private void Start()
    {
        if (actionMode == ButtonActionMode.MoveObject && targetObjectToMove != null)
        {
            originalPosition = targetObjectToMove.position;
            targetPosition = originalPosition + Vector3.up * moveDistance;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Button"))
            return;

        if (triggerOnlyOnce && hasBeenPressed)
            return;

        hasBeenPressed = true;
        Debug.Log("按钮已按下");

        switch (actionMode)
        {
            case ButtonActionMode.MoveObject:
                if (targetObjectToMove != null)
                    isMoving = true;
                break;

            case ButtonActionMode.HideObject:
                if (objectToHide != null)
                    objectToHide.SetActive(false);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!triggerOnlyOnce && other.CompareTag("Button"))
        {
            hasBeenPressed = false;
        }
    }

    private void Update()
    {
        if (isMoving && targetObjectToMove != null)
        {
            targetObjectToMove.position = Vector3.MoveTowards(
                targetObjectToMove.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(targetObjectToMove.position, targetPosition) < 0.001f)
            {
                isMoving = false;
            }
        }
    }
}
