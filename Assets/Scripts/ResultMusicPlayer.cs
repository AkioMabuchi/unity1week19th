using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultMusicPlayer : SingletonMonoBehaviour<ResultMusicPlayer>
{
    private AudioSource[] _audioSources;

    private void OnEnable()
    {
        _audioSources = GetComponents<AudioSource>();
    }

    public void StartMusicWin()
    {
        _audioSources[0].time = 0.0f;
        _audioSources[0].Play();
    }

    public void StartMusicLose()
    {
        _audioSources[1].time = 0.0f;
        _audioSources[1].Play();
    }
}
