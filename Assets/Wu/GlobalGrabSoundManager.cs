using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class GlobalGrabSoundManager : MonoBehaviour
{
    [Header("音效设置")]
    public AudioSource audioSource;   // 音频源组件
    public AudioClip grabSound;       // 抓取音效
    public AudioClip releaseSound;    // 释放音效
    public float volume = 1f;         // 音量大小

    [Header("音效选项")]
    public bool playOnGrab = true;    // 抓取时播放音效
    public bool playOnRelease = true; // 释放时播放音效

    private List<XRGrabInteractable> allGrabInteractables = new List<XRGrabInteractable>();

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

        // 查找场景中所有的XR Grab Interactable
        FindAllGrabInteractables();

        // 监听所有抓取事件
        SetupGrabListeners();
    }

    void FindAllGrabInteractables()
    {
        // 查找场景中所有的XR Grab Interactable
        XRGrabInteractable[] interactables = FindObjectsOfType<XRGrabInteractable>();
        allGrabInteractables.AddRange(interactables);

        Debug.Log($"找到 {allGrabInteractables.Count} 个可抓取物体");
    }

    void SetupGrabListeners()
    {
        foreach (var interactable in allGrabInteractables)
        {
            if (playOnGrab)
                interactable.selectEntered.AddListener(OnAnyGrab);
            if (playOnRelease)
                interactable.selectExited.AddListener(OnAnyRelease);
        }
    }

    void OnAnyGrab(SelectEnterEventArgs args)
    {
        Debug.Log($"抓取物体: {args.interactableObject.transform.name}");
        PlaySound(grabSound);
    }

    void OnAnyRelease(SelectExitEventArgs args)
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

    // 动态添加新的可抓取物体
    public void AddGrabInteractable(XRGrabInteractable interactable)
    {
        if (!allGrabInteractables.Contains(interactable))
        {
            allGrabInteractables.Add(interactable);
            if (playOnGrab)
                interactable.selectEntered.AddListener(OnAnyGrab);
            if (playOnRelease)
                interactable.selectExited.AddListener(OnAnyRelease);
        }
    }

    void OnDestroy()
    {
        // 清理所有事件监听
        foreach (var interactable in allGrabInteractables)
        {
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnAnyGrab);
                interactable.selectExited.RemoveListener(OnAnyRelease);
            }
        }
    }
}