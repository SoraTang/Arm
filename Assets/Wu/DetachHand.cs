using UnityEngine;

public class DetachedHand : MonoBehaviour
{
    public Transform leftController;
    private bool isFollowing = false;

    public void StartFollowing()
    {
        isFollowing = true;
    }

    void Update()
    {
        if (isFollowing)
        {
            transform.position = leftController.position;
            transform.rotation = leftController.rotation;
        }
    }
}
