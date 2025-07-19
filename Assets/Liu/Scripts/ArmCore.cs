// Runtime script: ArmCore.cs
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// ArmCore with detachable functionality using a property: creates a holder named "DetachedHand" at Start,
/// records the original parent, and toggles between original and detached holder when the
/// `Detached` property is set.
/// </summary>
[ExecuteAlways]
public class ArmCore : MonoBehaviour
{
    [Tooltip("Internal state for detach/reattach.")]
    [SerializeField]
    private bool detachedState = false;

    /// <summary>
    /// Gets or sets the detached state. Setting this property
    /// will automatically apply the detach or reattach operation.
    /// </summary>
    public bool Detached
    {
        get => detachedState;
        set
        {
            if (detachedState != value)
            {
                detachedState = value;
                ApplyDetachState(detachedState);
            }
        }
    }

    private Transform originalParent;
    private GameObject detachedHolder;

    private Vector3 defaultLocalPosition = new Vector3(0, 0.27404680f, 0);
    Quaternion defaultRotation = Quaternion.Euler(0, 35, 0);

    public InputActionReference detachAction;

    public XRGrabInteractable grabInteractable;
    
    void Start() => Initialize();

    void OnEnable() => Initialize();

    private void Initialize()
    {
        if (originalParent == null)
            originalParent = transform.parent;

        if (detachedHolder == null)
            detachedHolder = GameObject.Find("DetachedHand") ?? new GameObject("DetachedHand");

        ApplyDetachState(detachedState);
        
        if (detachAction != null)
        {
            detachAction.action.Enable();
            detachAction.action.performed += (context => {
                Detached = !Detached; // Toggle detach state on action performed
            });
        }

        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void Update()
    {
        if (grabInteractable.isSelected)
        {
            Detached = true;
            Debug.LogWarning("!!!");
        } else 
        {
            Debug.Log($"detached:{Detached}");
        }
    }

    private void ApplyDetachState(bool state)
    {
        if (transform == null)
            return;
        
        // if switching to detachedHolder, record transform's world position and move the holder here.
        if (state && detachedHolder != null)
        {
            detachedHolder.transform.position =
                transform.position - transform.parent.rotation * transform.localPosition;
            detachedHolder.transform.rotation = transform.rotation;
            // transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.rotation = defaultRotation;
            transform.localPosition = defaultLocalPosition;
        }
        
        transform.SetParent(state ? detachedHolder.transform : originalParent, true);
    }
    
    
    
}