using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GroupSettings : SingletonMonoBehaviour<GroupSettings>
{
    [SerializeField] private Sprite spriteSettingButtonAvatar;
    [SerializeField] private Sprite spriteSettingButtonAvatarSelected;
    
    [SerializeField] private GameObject gameObjectSliderMusic;
    [SerializeField] private GameObject gameObjectSliderSound;

    [SerializeField] private GameObject[] gameObjectsButtonsAvatar = new GameObject[2];

    [SerializeField] private GameObject gameObjectButtonDescriptionOn;
    [SerializeField] private GameObject gameObjectButtonDescriptionOff;

    private Slider _sliderMusic;
    private Slider _sliderSound;
    
    private readonly Button[] _buttonsAvatar = new Button[2];

    private Button _buttonDescriptionOn;
    private Button _buttonDescriptionOff;

    private readonly Subject<float> _onValueChangedMusic = new Subject<float>();
    private readonly Subject<float> _onValueChangedSound = new Subject<float>();
    private readonly Subject<int> _onClickAvatar = new Subject<int>();
    private readonly Subject<bool> _onClickDescription = new Subject<bool>();
    private readonly Subject<Unit> _onPointerUpSound = new Subject<Unit>();
    private readonly Subject<Unit> _onClickFinish = new Subject<Unit>();
    
    public IObservable<float> OnValueChangedMusic => _onValueChangedMusic;
    public IObservable<float> OnValueChangedSound => _onValueChangedSound;
    public IObservable<int> OnClickAvatar => _onClickAvatar;
    public IObservable<bool> OnClickDescription => _onClickDescription;
    public IObservable<Unit> OnPointerUpSound => _onPointerUpSound;
    public IObservable<Unit> OnClickFinish => _onClickFinish;

    private void OnEnable()
    {
        _sliderMusic = gameObjectSliderMusic.GetComponent<Slider>();
        _sliderSound = gameObjectSliderSound.GetComponent<Slider>();

        for (int i = 0; i < 2; i++)
        {
            _buttonsAvatar[i] = gameObjectsButtonsAvatar[i].GetComponent<Button>();
        }

        _buttonDescriptionOn = gameObjectButtonDescriptionOn.GetComponent<Button>();
        _buttonDescriptionOff = gameObjectButtonDescriptionOff.GetComponent<Button>();
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

    public void OnClickButtonAvatar(int id)
    {
        _onClickAvatar.OnNext(id);
    }

    public void OnClickButtonDescription(bool s)
    {
        _onClickDescription.OnNext(s);
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

    public void SetAvatarId(int id)
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == id)
            {
                _buttonsAvatar[i].image.sprite = spriteSettingButtonAvatarSelected;
            }
            else
            {
                _buttonsAvatar[i].image.sprite = spriteSettingButtonAvatar;
            }
        }
    }

    public void SetDescription(bool s)
    {
        if (s)
        {
            _buttonDescriptionOn.image.sprite = spriteSettingButtonAvatarSelected;
            _buttonDescriptionOff.image.sprite = spriteSettingButtonAvatar;
        }
        else
        {
            _buttonDescriptionOn.image.sprite = spriteSettingButtonAvatar;
            _buttonDescriptionOff.image.sprite = spriteSettingButtonAvatarSelected;
        }
    }
}
