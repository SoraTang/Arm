using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class MoveToGrab : MonoBehaviour
{
    [Header("Arm")]
    public ArmCore armCore;

    [Header("输入")] 
    public InputActionReference grabAction;    // Grab 动作
    public InputActionReference grabReleaseAction;    // Grab Release 动作

    [Header("Tween 参数")]
    public float duration = 2f;
    public Ease easeType = Ease.OutQuad;

    private Vector3 localHandOffset;   // 抓始时 Hand 的局部位移
    private Tweener grabTween;
    private GameObject grabbedGameObject = null;

    public Transform detachedHand;

    // 当前在 Trigger 范围内的可抓取物体
    private readonly List<Collider> overlaps = new List<Collider>();

    private void Start()
    {
    }

    private void Reset()
    {
        // 确保 Collider 是 Trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        // 绑定输入事件
        if (grabAction != null)
            grabAction.action.performed += OnGrabPerformed;
        grabAction?.action.Enable();
        if (grabReleaseAction != null)
            grabReleaseAction.action.performed += OnGrabReleased;
        grabReleaseAction?.action.Enable();
    }

    private void OnDisable()
    {
        // 解绑输入事件
        grabAction?.action.Disable();
        if (grabAction != null)
            grabAction.action.performed -= OnGrabPerformed;
        grabReleaseAction?.action.Disable();
        if (grabReleaseAction != null)
            grabReleaseAction.action.performed -= OnGrabReleased;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CanGrab") || other.CompareTag("CanGrabStatic"))
            overlaps.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CanGrab") || other.CompareTag("CanGrabStatic"))
            overlaps.Remove(other);
    }

    private void OnGrabPerformed(InputAction.CallbackContext ctx)
    {
        // 输入触发，手拆下时才抓取
        if (armCore.Detached)
        {
            TryGrabNearest();
            Rigidbody detachedRb = detachedHand.GetComponent<Rigidbody>();
            if (detachedRb != null)
            {
                // 设置 detachedHand 的 kinematic
                detachedRb.isKinematic = true;
            }
        }
    }

    private void OnGrabReleased(InputAction.CallbackContext ctx)
    {
        if (grabbedGameObject != null) {
            grabbedGameObject.transform.parent = null;
            overlaps.Remove(grabbedGameObject.GetComponent<Collider>());
            grabbedGameObject = null;
        }
        
        // 解除 detachedHand 的 kinematic
        if (detachedHand != null)
        {
            Rigidbody detachedRb = detachedHand.GetComponent<Rigidbody>();
            if (detachedRb != null)
            {
                detachedRb.isKinematic = false;
            }
        }

    }

    private void TryGrabNearest()
    {
        if (overlaps.Count == 0)
        {
            Debug.LogWarning("No object to grab");
            return;
        }

        // 找最近的那一个
        Collider nearest = null;
        float minDistSqr = float.MaxValue;
        Vector3 myPos = transform.position;

        foreach (var col in overlaps)
        {
            if (col == null) continue;
            float d2 = (col.transform.position - myPos).sqrMagnitude;
            if (d2 < minDistSqr)
            {
                minDistSqr = d2;
                nearest = col;
            }
        }

        if (nearest != null)
            MoveToTarget(nearest.transform);
    }

private void MoveToTarget(Transform target)
{
    // 1. 记录当前 Hand 的局部位移 (只需一次)
    localHandOffset = transform.localPosition;
    Debug.Log($"localHandOffset: {localHandOffset}");

    // 2. 终止旧 Tween
    grabTween?.Kill();

    // 3. 缓存启动时目标位置
    Vector3 startPos = target.position;

    // 4. 创建 Tween
    grabTween = DOTween
        .To(() => 0f, // 虚拟进度
            t =>
            {
                Vector3 idealPos = transform.position;
                target.position = Vector3.LerpUnclamped(startPos, idealPos, t);
            },
            1f, duration)
        .SetEase(easeType)
        .OnComplete(() =>
        {
            // ✅ 设置为手部子对象
            target.transform.parent = transform;

            // ✅ 重置位置到手部中心
            target.localPosition = Vector3.zero;
            target.localRotation = Quaternion.identity;

            // ✅ 清除 Rigidbody 运动
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            grabbedGameObject = target.gameObject;
        });
}

}
