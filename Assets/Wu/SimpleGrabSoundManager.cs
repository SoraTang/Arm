using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class SimpleGrabSoundManager : MonoBehaviour
{
    [Header("音效设置")]
    public AudioSource audioSource;   // 音频源组件
    public AudioClip grabSound;       // 抓取音效
    public AudioClip releaseSound;    // 释放音效
    public float volume = 1f;         // 音量大小

    [Header("音效选项")]
    public bool playOnGrab = true;    // 抓取时播放音效
    public bool playOnRelease = true; // 释放时播放音效

    private List<XRGrabInteractable> trackedInteractables = new List<XRGrabInteractable>();

    void Start()
    {
        // 如果没有指定AudioSource，创建一个
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 设置AudioSource属性
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        // 查找并监听所有现有的可抓取物体
        FindAndTrackAllGrabInteractables();

        // 延迟检查，确保所有物体都已加载
        InvokeRepeating(nameof(CheckForNewInteractables), 1f, 2f);
    }

    void FindAndTrackAllGrabInteractables()
    {
        XRGrabInteractable[] interactables = FindObjectsOfType<XRGrabInteractable>();

        foreach (var interactable in interactables)
        {
            if (!trackedInteractables.Contains(interactable))
            {
                AddInteractableListener(interactable);
                trackedInteractables.Add(interactable);
            }
        }

        Debug.Log($"已跟踪 {trackedInteractables.Count} 个可抓取物体");
    }

    void CheckForNewInteractables()
    {
        XRGrabInteractable[] interactables = FindObjectsOfType<XRGrabInteractable>();

        foreach (var interactable in interactables)
        {
            if (!trackedInteractables.Contains(interactable))
            {
                AddInteractableListener(interactable);
                trackedInteractables.Add(interactable);
                Debug.Log($"新发现可抓取物体: {interactable.name}");
            }
        }
    }

    void AddInteractableListener(XRGrabInteractable interactable)
    {
        if (playOnGrab)
            interactable.selectEntered.AddListener(OnGrab);
        if (playOnRelease)
            interactable.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log($"抓取物体: {args.interactableObject.transform.name}");
        PlaySound(grabSound);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log($"释放物体: {args.interactableObject.transform.name}");
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
        // 清理所有事件监听
        foreach (var interactable in trackedInteractables)
        {
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnGrab);
                interactable.selectExited.RemoveListener(OnRelease);
            }
        }
    }
}