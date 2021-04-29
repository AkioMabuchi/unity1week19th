using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GroupSettings : SingletonMonoBehaviour<GroupSettings>
{
    [SerializeField] private GameObject gameObjectSliderMusic;
    [SerializeField] private GameObject gameObjectSliderSound;

    private Slider _sliderMusic;
    private Slider _sliderSound;
    
    private readonly Subject<float> _onValueChangedMusic = new Subject<float>();
    private readonly Subject<float> _onValueChangedSound = new Subject<float>();
    private readonly Subject<Unit> _onPointerUpSound = new Subject<Unit>();
    private readonly Subject<Unit> _onClickFinish = new Subject<Unit>();
    
    public IObservable<float> OnValueChangedMusic => _onValueChangedMusic;
    public IObservable<float> OnValueChangedSound => _onValueChangedSound;

    public IObservable<Unit> OnPointerUpSound => _onPointerUpSound;
    public IObservable<Unit> OnClickFinish => _onClickFinish;

    private void OnEnable()
    {
        _sliderMusic = gameObjectSliderMusic.GetComponent<Slider>();
        _sliderSound = gameObjectSliderSound.GetComponent<Slider>();
    }

    public void OnValueChangedSliderMusic()
    {
        _onValueChangedMusic.OnNext(_sliderMusic.value);
    }

    public void OnValueChangedSliderSound()
    {
        _onValueChangedSound.OnNext(_sliderSound.value);
    }

    public void OnPointerUpSliderSound()
    {
        _onPointerUpSound.OnNext(Unit.Default);
    }
    
    public void OnClickButtonFinish()
    {
        _onClickFinish.OnNext(Unit.Default);
    }

    public void SetMusicVolume(float value)
    {
        _sliderMusic.value = value;
    }

    public void SetSoundVolume(float value)
    {
        _sliderSound.value = value;
    }
}
