using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class GroupFadeEffect : SingletonMonoBehaviour<GroupFadeEffect>
{
    [SerializeField] private GameObject gameObjectImage;

    private Image _image;
    
    private readonly Subject<Unit> _onEndFadeIn = new Subject<Unit>();
    private readonly Subject<Unit> _onEndFadeOut = new Subject<Unit>();
    public IObservable<Unit> OnEndFadeIn => _onEndFadeIn;
    public IObservable<Unit> OnEndFadeOut => _onEndFadeOut;

    private void OnEnable()
    {
        _image = gameObjectImage.GetComponent<Image>();
    }

    public void FadeIn()
    {
        gameObjectImage.SetActive(true);
        _image.DOColor(new Color(0.0f, 0.0f, 0.0f, 1.0f), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _onEndFadeIn.OnNext(Unit.Default);
        });
    }

    public void FadeOut()
    {
        _image.DOColor(new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObjectImage.SetActive(false);
            _onEndFadeOut.OnNext(Unit.Default);
        });
    }
}
