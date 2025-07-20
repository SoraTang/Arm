using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;

/// <summary>
/// ArmCore with detachable functionality and shoulder auto-reconnect.
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
    public MonoBehaviour detachedOnlyComponent;

    [Header("抓取时禁用的物体")]
    public GameObject objectToDisableWhenGrabbed;

    [Header("脱离手臂时碰撞器交换")]
    public Collider detachedArmCollider;
    public Collider defaultArmCollider;
    

    [Header("回收设置")]
    public float pullDuration = 0.2f;

    [Header("肩部触发器（必须是 IsTrigger 的 Collider）")]
    public Collider shoulderTriggerZone;

    [Header("手臂触发器（必须是 IsTrigger 的 Collider，用于检测是否进入肩部）")]
    public Collider triggerSourceCollider;

    private Transform originalParent;
    private GameObject detachedHolder;

    private Vector3 defaultLocalPosition = new Vector3(0, 0.27404680f, 0);
    private Quaternion defaultRotation = Quaternion.Euler(0, 35, 0);

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
            detachAction.action.performed += (context => Detached = !Detached);
        }

        if (callCallBackAction != null)
        {
            callCallBackAction.action.Enable();
            callCallBackAction.action.performed += (context => CallCallBack());
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
            detachedArmCollider.enabled = !Detached;
            defaultArmCollider.enabled = Detached;
        }
    }

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

        if (detachedOnlyComponent != null)
            detachedOnlyComponent.enabled = state;
    }

    public void CallCallBack()
    {
        transform.rotation = defaultRotation;

        transform.parent.DOMove(originalParent.position, pullDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Detached = false);
    }

    /// <summary>
    /// 检测肩部触发器是否与指定触发器发生碰撞（仅当 detached 状态）
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (shoulderTriggerZone == null || triggerSourceCollider == null)
            return;

        // 只有当肩膀触发区域是“other”，并且进入的是我们指定的 triggerSourceCollider
        if (other == shoulderTriggerZone && triggerSourceCollider.bounds.Intersects(shoulderTriggerZone.bounds))
        {
            if (Detached && !grabInteractable.isSelected)
            {
                Debug.Log("进入肩部区域，自动回收 CallCallBack()");
                CallCallBack();
            }
        }
    }
}
