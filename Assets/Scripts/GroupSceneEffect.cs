using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UniRx;

public class GroupSceneEffect : SingletonMonoBehaviour<GroupSceneEffect>
{
    [SerializeField] private GameObject gameObjectImageEffect;

    private readonly Subject<Unit> _onCompleteHide = new Subject<Unit>();
    private readonly Subject<Unit> _onCompleteShow = new Subject<Unit>();
    public IObservable<Unit> OnCompleteHide => _onCompleteHide;
    public IObservable<Unit> OnCompleteShow => _onCompleteShow;

    public void Hide()
    {
        gameObjectImageEffect.transform.DOLocalMoveX(0.0f, 1.0f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            _onCompleteHide.OnNext(Unit.Default);
        });
    }

    public void Show()
    {
        gameObjectImageEffect.transform.DOLocalMoveX(320.0f, 1.0f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            _onCompleteShow.OnNext(Unit.Default);
        });
    }
}
