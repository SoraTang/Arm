using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabSoundController : MonoBehaviour
{
    [Header("音效设置")]
    public AudioSource audioSource;   // 音频源组件
    public AudioClip grabSound;       // 抓取音效
    public AudioClip releaseSound;    // 释放音效
    public float volume = 1f;         // 音量大小

    [Header("音效选项")]
    public bool playOnGrab = true;    // 抓取时播放音效
    public bool playOnRelease = true; // 释放时播放音效

    private XRGrabInteractable grabInteractable;

    void Start()
    {
        // 获取XR Grab Interactable组件
        grabInteractable = GetComponent<XRGrabInteractable>();

        // 如果没有指定AudioSource，尝试获取当前物体上的AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // 如果没有AudioSource，创建一个
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 设置AudioSource属性
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        // 监听抓取事件
        if (grabInteractable != null)
        {
            if (playOnGrab)
                grabInteractable.selectEntered.AddListener(OnGrab);
            if (playOnRelease)
                grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("物体被抓取");
        PlaySound(grabSound);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("物体被释放");
        PlaySound(releaseSound);
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

    void OnDestroy()
    {
        // 清理事件监听
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}