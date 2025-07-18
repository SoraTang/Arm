using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;               // VR 设备位置（头/手）
    public Transform ikTarget;               // IK 目标（驱动模型骨骼）
    public Vector3 trackingPositionOffset;   // 位置偏移
    public Vector3 trackingRotationOffset;   // 旋转偏移

    public void Map()
    {
        ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class IKTargetFollowVRRig : MonoBehaviour
{
    [Range(0f, 1f)]
    public float turnSmoothness = 0.1f;

    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Vector3 headBodyPositionOffset;
    public float headBodyYawOffset = 180f; // 默认偏移 180 度解决模型朝向相反问题

    private void Start()
    {
        // 可选：根据初始朝向动态设置偏移，避免手动调节
        // headBodyYawOffset = transform.eulerAngles.y - head.vrTarget.eulerAngles.y;
    }

    void LateUpdate()
    {
        // 设置角色根位置（通常是角色身体）
        transform.position = head.ikTarget.position + headBodyPositionOffset;

        // 获取头显的 Y 轴朝向 + 偏移值
        float yaw = head.vrTarget.eulerAngles.y + headBodyYawOffset;

        // 平滑插值调整模型旋转朝向
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSmoothness);

        // 映射头和双手的位置和旋转
        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
