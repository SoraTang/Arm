using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControllerMotionForce : MonoBehaviour
{
    [Header("Left Controller Tracking")]
    public Transform leftController;  // 直接引用 XR 左手控制器的 Transform（如XR Origin下的子物体）

    [Header("Force Settings")]
    public float velocityThreshold = 0.5f;
    public float forceMultiplier = 10f;

    private Rigidbody rb;
    private Vector3 lastPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (leftController != null)
            lastPosition = leftController.position;
    }

    private void FixedUpdate()
    {
        if (leftController == null)
            return;

        Vector3 currentPosition = leftController.position;
        Vector3 velocity = (currentPosition - lastPosition) / Time.fixedDeltaTime;

        // ✅ Debug 输出速度和方向
        Debug.Log($"Controller velocity: {velocity}, Magnitude: {velocity.magnitude}");

        if (velocity.magnitude > velocityThreshold)
        {
            Vector3 force = velocity.normalized * (velocity.magnitude - velocityThreshold) * forceMultiplier;
            rb.AddForce(force, ForceMode.Impulse);
        }

        lastPosition = currentPosition;
    }
}
