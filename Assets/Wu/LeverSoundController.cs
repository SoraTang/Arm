using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class LeverSoundController : MonoBehaviour
{
    [Header("杠杆设置")]
    public XRLever lever;

    [Header("音效设置")]
    public AudioSource audioSource;
    public AudioClip leverOnSound;    // 杠杆开启音效
    public AudioClip leverOffSound;   // 杠杆关闭音效
    public float volume = 1f;

    [Header("音效选项")]
    public bool playOnActivate = true;    // 激活时播放音效
    public bool playOnDeactivate = true;  // 停用时播放音效
    public bool playOnGrab = false;       // 抓取时播放音效
    public bool playOnRelease = false;    // 释放时播放音效

    void Start()
    {
        if (lever == null)
        {
            lever = GetComponent<XRLever>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (lever != null)
        {
            // 监听杠杆状态变化
            if (playOnActivate)
                lever.onLeverActivate.AddListener(OnLeverActivate);
            if (playOnDeactivate)
                lever.onLeverDeactivate.AddListener(OnLeverDeactivate);
            if (playOnGrab)
                lever.selectEntered.AddListener(OnGrabStart);
            if (playOnRelease)
                lever.selectExited.AddListener(OnGrabEnd);
        }
    }

    void OnLeverActivate()
    {
        PlaySound(leverOnSound);
    }

    void OnLeverDeactivate()
    {
        PlaySound(leverOffSound);
    }

    void OnGrabStart(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        PlaySound(leverOnSound);
    }

    void OnGrabEnd(UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs args)
    {
        PlaySound(leverOffSound);
    }

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
        if (lever != null)
        {
            lever.onLeverActivate.RemoveListener(OnLeverActivate);
            lever.onLeverDeactivate.RemoveListener(OnLeverDeactivate);
            lever.selectEntered.RemoveListener(OnGrabStart);
            lever.selectExited.RemoveListener(OnGrabEnd);
        }
    }
}