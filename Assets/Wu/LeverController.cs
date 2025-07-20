using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class LeverController : MonoBehaviour
{
    [Header("杠杆设置")]
    public XRLever lever1;           // 第一个杠杆
    public XRLever lever2;           // 第二个杠杆

    [Header("移动物体设置")]
    public Transform targetObject;    // 要移动的物体
    public float moveSpeed = 2f;      // 移动速度
    public float maxHeight = 5f;      // 最大高度
    public float minHeight = 0f;      // 最小高度

    [Header("音效设置")]
    public AudioSource audioSource;   // 音频源组件
    public AudioClip leverOnSound;    // 杠杆开启音效
    public AudioClip leverOffSound;   // 杠杆关闭音效
    public float volume = 1f;         // 音量大小

    private bool isMoving = false;
    private Vector3 originalPosition;

    void Start()
    {
        if (targetObject != null)
        {
            originalPosition = targetObject.position;
        }

        // 如果没有指定AudioSource，尝试获取当前物体上的AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // 监听杠杆状态变化（用于控制物体移动）
        if (lever1 != null)
        {
            lever1.onLeverActivate.AddListener(OnLeverStateChanged);
            lever1.onLeverDeactivate.AddListener(OnLeverStateChanged);
            // 监听杠杆1的音效事件
            lever1.onLeverActivate.AddListener(OnLever1Activate);
            lever1.onLeverDeactivate.AddListener(OnLever1Deactivate);
        }

        if (lever2 != null)
        {
            lever2.onLeverActivate.AddListener(OnLeverStateChanged);
            lever2.onLeverDeactivate.AddListener(OnLeverStateChanged);
            // 监听杠杆2的音效事件
            lever2.onLeverActivate.AddListener(OnLever2Activate);
            lever2.onLeverDeactivate.AddListener(OnLever2Deactivate);
        }

        // 初始化时检查杠杆状态
        Invoke(nameof(CheckInitialState), 0.1f);
    }

    void CheckInitialState()
    {
        OnLeverStateChanged();
    }

    void OnLeverStateChanged()
    {
        // 检查两个杠杆是否都激活
        bool bothActive = (lever1 != null && lever1.value) && (lever2 != null && lever2.value);

        if (bothActive)
        {
            isMoving = true;
            Debug.Log("两个杠杆都激活，开始向上移动");
        }
        else
        {
            isMoving = false;
            Debug.Log("杠杆状态改变，停止移动");
            Debug.Log("lever1.value: " + (lever1 != null ? lever1.value.ToString() : "null"));
            Debug.Log("lever2.value: " + (lever2 != null ? lever2.value.ToString() : "null"));
        }
    }

    // 杠杆1激活事件
    void OnLever1Activate()
    {
        Debug.Log("杠杆1激活");
        PlaySound(leverOnSound);
    }

    // 杠杆1停用事件
    void OnLever1Deactivate()
    {
        Debug.Log("杠杆1停用");
        PlaySound(leverOffSound);
    }

    // 杠杆2激活事件
    void OnLever2Activate()
    {
        Debug.Log("杠杆2激活");
        PlaySound(leverOnSound);
    }

    // 杠杆2停用事件
    void OnLever2Deactivate()
    {
        Debug.Log("杠杆2停用");
        PlaySound(leverOffSound);
    }

    void Update()
    {
        if (targetObject == null || !isMoving) return;

        // 向上移动物体
        Vector3 currentPos = targetObject.position;
        float newY = Mathf.MoveTowards(currentPos.y, maxHeight, moveSpeed * Time.deltaTime);
        targetObject.position = new Vector3(currentPos.x, newY, currentPos.z);

        // 如果到达最大高度，停止移动
        if (Mathf.Approximately(newY, maxHeight))
        {
            isMoving = false;
        }
    }

    // 播放音效的方法
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    // 重置物体位置的方法
    public void ResetObjectPosition()
    {
        if (targetObject != null)
        {
            targetObject.position = originalPosition;
        }
    }

    void OnDestroy()
    {
        // 清理事件监听
        if (lever1 != null)
        {
            lever1.onLeverActivate.RemoveListener(OnLeverStateChanged);
            lever1.onLeverDeactivate.RemoveListener(OnLeverStateChanged);
            lever1.onLeverActivate.RemoveListener(OnLever1Activate);
            lever1.onLeverDeactivate.RemoveListener(OnLever1Deactivate);
        }

        if (lever2 != null)
        {
            lever2.onLeverActivate.RemoveListener(OnLeverStateChanged);
            lever2.onLeverDeactivate.RemoveListener(OnLeverStateChanged);
            lever2.onLeverActivate.RemoveListener(OnLever2Activate);
            lever2.onLeverDeactivate.RemoveListener(OnLever2Deactivate);
        }
    }
}