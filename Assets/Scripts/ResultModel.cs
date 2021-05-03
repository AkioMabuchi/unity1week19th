using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public enum GameResult
{
    Draw,
    Win,
    Lose
}
public class ResultModel : SingletonMonoBehaviour<ResultModel>
{
    private readonly ReactiveProperty<GameResult> _result = new ReactiveProperty<GameResult>(GameResult.Draw);

    public IObservable<GameResult> Result => _result;

    public void SetResult(GameResult result)
    {
        _result.Value = result;
    }
}
