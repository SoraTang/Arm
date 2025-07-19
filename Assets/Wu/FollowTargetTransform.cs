using UnityEngine;

public class FollowTargetTransform : MonoBehaviour
{
    public Transform target; // 需要跟随的目标物体

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}