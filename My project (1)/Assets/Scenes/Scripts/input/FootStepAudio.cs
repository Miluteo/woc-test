using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioClip[] footstepSounds;
    public float volume = 0.5f;

    private AudioSource audioSource;
    private int currentIndex = 0;  // 记录当前该播放哪个音效

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayFootSound()
    {
        if (footstepSounds.Length > 0)
        {
            // 播放当前索引的音效
            audioSource.PlayOneShot(footstepSounds[currentIndex], volume);

            // 索引加1，如果到末尾就回到0
            currentIndex++;
            if (currentIndex >= footstepSounds.Length)
                currentIndex = 0;
        }
    }
}