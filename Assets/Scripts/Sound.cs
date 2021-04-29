using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private AudioClip _audioClip;
    
    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _audioSource.clip = _audioClip; 
        _audioSource.time = 0.0f;
        _audioSource.Play();
        StartCoroutine(CoroutineSound());
    }

    IEnumerator CoroutineSound()
    {
        while (_audioSource.isPlaying)
        {
            yield return null;
        }

        Destroy(gameObject);
    }

    public void Initialize(AudioClip audioClip)
    {
        _audioClip = audioClip;
    }
}
