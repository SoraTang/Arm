using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCollisionPrevention : MonoBehaviour
{
    public Transform xrRig; // XR Rig整体
    private Vector3 lastSafePosition;

    void Start()
    {
        lastSafePosition = xrRig.position;
    }

    void Update()
    {
        // 检查头部是否碰到障碍物
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.15f, LayerMask.GetMask("Wall")); // "Wall"为障碍物Layer
        if (hits.Length > 0)
        {
            xrRig.position = lastSafePosition; // 回退
        }
        else
        {
            lastSafePosition = xrRig.position;
        }
    }
}
