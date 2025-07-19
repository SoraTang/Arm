using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandDetachController : MonoBehaviour
{
    public Transform leftUpperArm;
    public Transform leftLowerArm;
    public Transform leftHand;
    public Transform ikTarget;  // 跟踪控制器的 LeftHandTarget
    public Transform leftController;
    public float detachThreshold = 0.4f;
    public bool isDetached = false;

    public GameObject detachedArmPrefab; // 拆下来的 forearm + hand

    void Update()
    {
        if (isDetached) return;

        float dist = Vector3.Distance(ikTarget.position, leftUpperArm.position);
        if (dist > detachThreshold)
        {
            DetachForearm();
        }
    }

    void DetachForearm()
    {
        isDetached = true;

        // 1. 禁用 IK
        GetComponent<TwoBoneIKConstraint>().weight = 0;

        // 2. 禁用原始模型上的前臂 + 手（避免重叠）s
        leftLowerArm.gameObject.SetActive(false);

        // 3. 实例化 detached 物体
        GameObject detached = Instantiate(detachedArmPrefab, leftLowerArm.position, leftLowerArm.rotation);

        // 4. 将控制器传给它
        DetachedHand detachedScript = detached.GetComponent<DetachedHand>();
        detachedScript.leftController = leftController;
        detachedScript.StartFollowing();
    }
}

