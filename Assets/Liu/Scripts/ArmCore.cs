using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;

/// <summary>
/// ArmCore with detachable functionality:
/// creates a holder named "DetachedHand" at Start,
/// records the original parent, and toggles between original and detached holder when the
/// `Detached` property is set. Also activates/deactivates a specified component based on detach state.
/// </summary>
[ExecuteAlways]
public class ArmCore : MonoBehaviour
{
    [Tooltip("Internal state for detach/reattach.")]
    [SerializeField]
    private bool detachedState = false;

    [Header("回收输入")] 
    public InputActionReference callCallBackAction;

    [Header("脱离控制输入")]
    public InputActionReference detachAction;

    [Header("XR绑定组件")]
    public XRGrabInteractable grabInteractable;
    public FollowTargetTransform followTarget;
    public Rigidbody fakeArmRigidbody;

    [Header("脱离状态激活组件")]
    public MonoBehaviour detachedOnlyComponent;  // ✅ 将在脱离时启用，附着时禁用

    [Header("回收设置")]
    public float pullDuration = 0.2f;

    [Header("抓取时禁用的物体")]
public GameObject objectToDisableWhenGrabbed;

    private Transform originalParent;
    private GameObject detachedHolder;

    private Vector3 defaultLocalPosition = new Vector3(0, 0.27404680f, 0);
    private Quaternion defaultRotation = Quaternion.Euler(0, 35, 0);

    /// <summary>
    /// Gets or sets the detached state. Setting this property
    /// will automatically apply the detach or reattach operation.
    /// </summary>
    public bool Detached
    {
        get => detachedState;
        set
        {
            detachedState = value;
            ApplyDetachState(detachedState);
        }
    }

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
                Detached = !Detached;
            });
        }

        if (callCallBackAction != null)
        {
            callCallBackAction.action.Enable();
            callCallBackAction.action.performed += (context => {
                CallCallBack();
            });
        }
    }

void Update()
{
    if (grabInteractable.isSelected)
    {
        Detached = true;
        if (objectToDisableWhenGrabbed != null)
            objectToDisableWhenGrabbed.SetActive(false);
    }
    else
    {
        fakeArmRigidbody.useGravity = Detached;
        if (objectToDisableWhenGrabbed != null)
            objectToDisableWhenGrabbed.SetActive(true);
    }
}

    /// <summary>
    /// Applies the visual and logical state when switching between detached/attached.
    /// </summary>
    private void ApplyDetachState(bool state)
    {
        if (transform == null)
            return;

        if (state && detachedHolder != null)
        {
            followTarget.enabled = false;
            detachedHolder.transform.position =
                transform.position - transform.parent.rotation * transform.localPosition;
            detachedHolder.transform.rotation = transform.rotation;
        }
        else
        {
            transform.rotation = defaultRotation;
            transform.localPosition = defaultLocalPosition;
            followTarget.enabled = true;
        }

        transform.SetParent(state ? detachedHolder.transform : originalParent, true);

        // ✅ 启用/禁用指定组件
        if (detachedOnlyComponent != null)
        {
            detachedOnlyComponent.enabled = state;
        }
    }

    /// <summary>
    /// Animates the arm back to its original position and reattaches it.
    /// </summary>
    public void CallCallBack()
    {
        transform.rotation = defaultRotation;

        transform.parent.DOMove(originalParent.position, pullDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Detached = false);
    }
}
