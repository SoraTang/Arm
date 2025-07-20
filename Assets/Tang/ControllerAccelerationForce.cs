using UnityEngine;

public class ControllerMotionForce : MonoBehaviour
{
    [Header("Left Controller Tracking")]
    public Transform leftController;

    [Header("Target to Apply Force")]
    public Rigidbody targetRigidbody;
    public Collider targetCollider;

    [Header("Force Settings")]
    public float velocityThreshold = 0.5f;
    public float forceMultiplier = 10f;

    [Header("Ground Check")]
    public float checkRadius = 0.1f;
    public LayerMask wallLayerMask; // 设置为只包含 Wall Layer

    private Vector3 lastPosition;

    private void Start()
    {
        if (leftController != null)
        {
            lastPosition = leftController.position;
        }
    }

    private void FixedUpdate()
    {
        if (leftController == null || targetRigidbody == null || targetCollider == null)
            return;

        // Step 1: 检查是否接触 Wall
        bool touchingWall = Physics.CheckSphere(targetCollider.bounds.center, checkRadius, wallLayerMask);

        // Step 2: 若未接触 Wall，直接退出
        if (!touchingWall)
            return;

        // Step 3: 计算控制器速度
        Vector3 currentPosition = leftController.position;
        Vector3 velocity = (currentPosition - lastPosition) / Time.fixedDeltaTime;


        if (velocity.magnitude > velocityThreshold)
        {
            Vector3 force = velocity.normalized * (velocity.magnitude - velocityThreshold) * forceMultiplier;
            targetRigidbody.AddForce(force, ForceMode.Impulse);
        }

        lastPosition = currentPosition;
    }
}
