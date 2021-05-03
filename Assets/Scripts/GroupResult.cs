using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GroupResult : SingletonMonoBehaviour<GroupResult>
{
    [SerializeField] private Sprite[] spritesAvatar;
    
    [SerializeField] private GameObject gameObjectImageBackground;
    [SerializeField] private GameObject gameObjectImageAvatar;
    [SerializeField] private GameObject gameObjectTextMeshProResult;
    [SerializeField] private GameObject gameObjectButtonTitle;
    [SerializeField] private GameObject gameObjectButtonPrepare;
    [SerializeField] private GameObject gameObjectButtonRetry;

    private Image _imageBackground;
    private Image _imageAvatar;
    private TextMeshProUGUI _textMeshProResult;

    private readonly Subject<Unit> _onClickTitle = new Subject<Unit>();
    private readonly Subject<Unit> _onClickPrepare = new Subject<Unit>();
    private readonly Subject<Unit> _onClickRetry = new Subject<Unit>();
    public IObservable<Unit> OnClickTitle => _onClickTitle;
    public IObservable<Unit> OnClickPrepare => _onClickPrepare;
    public IObservable<Unit> OnClickRetry => _onClickRetry;
    private void OnEnable()
    {
        _imageBackground = gameObjectImageBackground.GetComponent<Image>();
        _imageAvatar = gameObjectImageAvatar.GetComponent<Image>();
        _textMeshProResult = gameObjectTextMeshProResult.GetComponent<TextMeshProUGUI>();
    }

    public void DrawResult(GameResult result)
    {
        switch (result)
        {
            case GameResult.Draw:
                _imageBackground.color = new Color(0.3f, 0.3f, 0.3f);
                _textMeshProResult.text = "引き分け";
                _textMeshProResult.color = new Color(1.0f, 1.0f, 1.0f);
                break;
            case GameResult.Win:
                _imageBackground.color = new Color(0.75f, 0.375f, 0.75f);
                _textMeshProResult.text = "勝利!!";
                _textMeshProResult.color = new Color(1.0f, 0.5f, 0.0f);
                break;
            case GameResult.Lose:
                _imageBackground.color = new Color(0.125f, 0.125f, 0.5f);
                _textMeshProResult.text = "敗北・・・";
                _textMeshProResult.color = new Color(0.5f, 0.75f, 1.0f);
                break;
        }
    }

    public void DrawAvatar(int avatarId)
    {
        _imageAvatar.sprite = spritesAvatar[avatarId];
    }

    public void ShowButtons()
    {
        gameObjectButtonTitle.SetActive(true);
        gameObjectButtonPrepare.SetActive(true);
        gameObjectButtonRetry.SetActive(true);
    }

    public void OnClickButtonTitle()
    {
        _onClickTitle.OnNext(Unit.Default);
    }

    public void OnClickButtonPrepare()
    {
        _onClickPrepare.OnNext(Unit.Default);
    }

    public void OnClickButtonRetry()
    {
        _onClickRetry.OnNext(Unit.Default);
    }
}
