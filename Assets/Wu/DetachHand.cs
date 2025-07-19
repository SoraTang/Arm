using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DetachedHand : MonoBehaviour
{
    public Transform objectToAttach; // 要变为子物体的对象
    public void OnSelectEnter(XRBaseInteractor interactor)
    {
        if (objectToAttach != null)
        {
            objectToAttach.SetParent(transform);
        }
        Debug.Log("OnSelectEnter");
    }
}