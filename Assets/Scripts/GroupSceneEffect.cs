using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GroupSceneEffect : SingletonMonoBehaviour<GroupSceneEffect>
{
    [SerializeField] private GameObject gameObjectImageEffect;

    private Coroutine _coroutineHide;
    private Coroutine _coroutineShow;

    private readonly Subject<Unit> _onCompleteHide = new Subject<Unit>();
    private readonly Subject<Unit> _onCompleteShow = new Subject<Unit>();
    public IObservable<Unit> OnCompleteHide => _onCompleteHide;
    public IObservable<Unit> OnCompleteShow => _onCompleteShow;

    public void Hide()
    {
        _coroutineHide = StartCoroutine(CoroutineHide());
    }

    public void Show()
    {
        _coroutineShow = StartCoroutine(CoroutineShow());
    }

    public void StopHide()
    {
        if (_coroutineHide != null)
        {
            StopCoroutine(_coroutineHide);
        }
    }

    public void StopShow()
    {
        if (_coroutineShow != null)
        {
            StopCoroutine(_coroutineShow);
        }
    }

    IEnumerator CoroutineHide()
    {
        int positionX = -320;
        while (positionX < 0)
        {
            yield return new WaitForFixedUpdate();
            positionX += 5;
            gameObjectImageEffect.transform.localPosition = new Vector3(positionX, 0.0f, 0.0f);
        }
        _onCompleteHide.OnNext(Unit.Default);
    }

    IEnumerator CoroutineShow()
    {
        int positionX = 0;
        while (positionX < 320)
        {
            yield return new WaitForFixedUpdate();
            positionX += 5;
            gameObjectImageEffect.transform.localPosition = new Vector3(positionX, 0.0f, 0.0f);
        }
        _onCompleteShow.OnNext(Unit.Default);
    }
}
