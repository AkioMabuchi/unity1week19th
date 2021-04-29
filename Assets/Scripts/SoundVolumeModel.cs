using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UniRx;

public class SoundVolumeModel : SingletonMonoBehaviour<SoundVolumeModel>
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private string musicVolumeKey;
    [SerializeField] private string soundVolumeKey;
    
    private readonly ReactiveProperty<float> _musicVolume = new ReactiveProperty<float>(0.0f);
    private readonly ReactiveProperty<float> _soundVolume = new ReactiveProperty<float>(0.0f);

    public IObservable<float> MusicVolume => _musicVolume;
    public IObservable<float> SoundVolume => _soundVolume;

    // Start is called before the first frame update
    void Start()
    {

        if (ES3.KeyExists(musicVolumeKey))
        {
            _musicVolume.Value = ES3.Load<float>(musicVolumeKey);
        }
        else
        {
            ES3.Save(musicVolumeKey, 0.0f);
        }

        if (ES3.KeyExists(soundVolumeKey))
        {
            _soundVolume.Value = ES3.Load<float>(soundVolumeKey);
        }
        else
        {
            ES3.Save(soundVolumeKey, 0.0f);
        }

        _musicVolume.Subscribe(musicVolume =>
        {
            float volume = musicVolume > -40.0f ? musicVolume : -80.0f;
            audioMixer.SetFloat("Music", volume);
            ES3.Save(musicVolumeKey, musicVolume);
        });

        _soundVolume.Subscribe(soundVolume =>
        {
            float volume = soundVolume > -40.0f ? soundVolume : -80.0f;
            audioMixer.SetFloat("Sound", volume);
            ES3.Save(soundVolumeKey, soundVolume);
        });
    }

    public void SetMusicVolume(float value)
    {
        _musicVolume.Value = value;
    }

    public void SetSoundVolume(float value)
    {
        _soundVolume.Value = value;
    }
}
