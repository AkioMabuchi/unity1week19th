using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class GroupTitle : SingletonMonoBehaviour<GroupTitle>
{
    private readonly Subject<Unit> _onClickStart = new Subject<Unit>();
    private readonly Subject<Unit> _onClickSettings = new Subject<Unit>();

    public IObservable<Unit> OnClickStart => _onClickStart;
    public IObservable<Unit> OnClickSettings => _onClickSettings;
    
    public void OnClickButtonStart()
    {
        _onClickStart.OnNext(Unit.Default);
    }

    public void OnClickButtonSettings()
    {
        _onClickSettings.OnNext(Unit.Default);
    }
}
