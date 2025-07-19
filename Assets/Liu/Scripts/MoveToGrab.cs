using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class MoveToGrab : MonoBehaviour
{
    [Header("输入")] 
    public InputActionReference grabAction;    // Grab 动作

    [Header("Tween 参数")]
    public float duration = 2f;
    public Ease easeType = Ease.OutQuad;

    private Vector3 localHandOffset;   // 抓始时 Hand 的局部位移
    private Tweener grabTween;

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
    }

    private void OnDisable()
    {
        // 解绑输入事件
        grabAction?.action.Disable();
        if (grabAction != null)
            grabAction.action.performed -= OnGrabPerformed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CanGrab"))
            overlaps.Add(other);
        Debug.Log("OnTriggerEnter: " + other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CanGrab"))
            overlaps.Remove(other);
        Debug.Log($"OnTriggerExit: {other.name}, Remaining: {overlaps.Count}");
    }

    private void OnGrabPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("GRAB!");
        // 输入触发时才抓取
        TryGrabNearest();
    }

    private void TryGrabNearest()
    {
        if (overlaps.Count == 0)
        {
            Debug.LogWarning("No objects to grab");
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

        // 3. 缓存启动时 forearm 位置
        Transform detachedHand = transform.parent.parent;
        Vector3 startPos = detachedHand.position;

        // 4. 创建 Tween
        grabTween = DOTween
            .To(() => 0f, // 虚拟进度
                t =>
                {
                    // 每帧根据最新 ForeArm 旋转重新计算理想位置
                    Vector3 idealPos = target.position - transform.position + detachedHand.position;

                    // 插值到理想位置。Unclamped 可使曲线外推更平滑
                    detachedHand.position = Vector3.LerpUnclamped(startPos, idealPos, t);
                },
                1f, duration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                // 强校正：再次计算并写死
                Vector3 idealPos = target.position - transform.position + detachedHand.position;
                detachedHand.position = idealPos;
            });
    }
}
