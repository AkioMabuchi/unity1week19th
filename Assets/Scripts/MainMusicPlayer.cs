using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MainMusicPlayer : SingletonMonoBehaviour<MainMusicPlayer>
{
    private AudioSource _audioSource;
    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
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
