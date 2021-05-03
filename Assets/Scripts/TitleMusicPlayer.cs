using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TitleMusicPlayer : SingletonMonoBehaviour<TitleMusicPlayer>
{
    private AudioSource _audioSource;
    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartMusic();
    }

    private void Update()
    {
        if (_audioSource.isPlaying)
        {
            if (_audioSource.time > 36.0f)
            {
                _audioSource.time = 4.0f;
            }
        }
    }

    public void StartMusic()
    {
        Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe(_ =>
        {
            _audioSource.time = 0.0f;
            _audioSource.Play();
        });
    }

    public void StopMusic()
    {
        _audioSource.Pause();
    }
}
