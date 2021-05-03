using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GroupConnection : SingletonMonoBehaviour<GroupConnection>
{
    [SerializeField] private Sprite[] spritesAvatar;
    [SerializeField] private GameObject gameObjectImageBackground;
    
    [SerializeField] private GameObject gameObjectButtonReturn;
    [SerializeField] private GameObject gameObjectButtonReconnect;
    
    [SerializeField] private GameObject gameObjectTextMeshProServerConnection;
    [SerializeField] private GameObject gameObjectTextMeshProMatchMaking;

    [SerializeField] private GameObject gameObjectTextMeshProOpponentFound;
    [SerializeField] private GameObject gameObjectTextMeshProOpponentName;
    [SerializeField] private GameObject gameObjectTextMeshProOpponentGreeting;
    [SerializeField] private GameObject gameObjectImageOpponentAvatar;

    [SerializeField] private GameObject gameObjectTextMeshProCountDown;
    
    private TextMeshProUGUI _textMeshProServerConnection;
    private TextMeshProUGUI _textMeshProMatchMaking;

    private TextMeshProUGUI _textMeshProOpponentName;
    private TextMeshProUGUI _textMeshProOpponentGreeting;
    private Image _imageOpponentAvatar;

    private TextMeshProUGUI _textMeshProCountDown;
    
    private readonly Subject<Unit> _onClickReturn = new Subject<Unit>();
    private readonly Subject<Unit> _onClickReconnection = new Subject<Unit>();
    public IObservable<Unit> OnClickReturn => _onClickReturn;
    public IObservable<Unit> OnClickReconnection => _onClickReconnection;

    private void OnEnable()
    {
        _textMeshProServerConnection = gameObjectTextMeshProServerConnection.GetComponent<TextMeshProUGUI>();
        _textMeshProMatchMaking = gameObjectTextMeshProMatchMaking.GetComponent<TextMeshProUGUI>();

        _textMeshProOpponentName = gameObjectTextMeshProOpponentName.GetComponent<TextMeshProUGUI>();
        _textMeshProOpponentGreeting = gameObjectTextMeshProOpponentGreeting.GetComponent<TextMeshProUGUI>();
        _imageOpponentAvatar = gameObjectImageOpponentAvatar.GetComponent<Image>();

        _textMeshProCountDown = gameObjectTextMeshProCountDown.GetComponent<TextMeshProUGUI>();
    }

    public void OnClickButtonReturn()
    {
        _onClickReturn.OnNext(Unit.Default);
    }

    public void OnClickButtonReconnection()
    {
        _onClickReconnection.OnNext(Unit.Default);
    }

    public void HideView()
    {
        gameObjectImageBackground.SetActive(false);
    }
    
    public void ShowServerConnection()
    {
        gameObjectTextMeshProServerConnection.SetActive(true);
    }

    public void DrawServerConnection(int count)
    {
        string[] texts =
        {
            "サーバーに接続中",
            "サーバーに接続中・",
            "サーバーに接続中・・",
            "サーバーに接続中・・・",
        };

        _textMeshProServerConnection.text = texts[count % 4];
    }
    
    public void HideServerConnection()
    {
        gameObjectTextMeshProServerConnection.SetActive(false);
    }

    public void ShowMatchMaking()
    {
        gameObjectTextMeshProMatchMaking.SetActive(true);
    }

    public void DrawMatchMaking(int count)
    {
        string[] texts =
        {
            "対戦相手を探しています",
            "対戦相手を探しています・",
            "対戦相手を探しています・・",
            "対戦相手を探しています・・・",
        };
        
        _textMeshProMatchMaking.text = texts[count % 4];
    }

    public void HideMatchMaking()
    {
        gameObjectTextMeshProMatchMaking.SetActive(false);
    }

    public void DisableReturnButton()
    {
        gameObjectButtonReturn.SetActive(false);
    }

    public void EnableReconnectButton()
    {
        gameObjectButtonReconnect.SetActive(true);
    }
    public void DisableReconnectButton()
    {
        gameObjectButtonReconnect.SetActive(false);
    }

    public void ShowOpponentInformation(string userName, int avatarId)
    {
        gameObjectTextMeshProOpponentFound.SetActive(true);
        gameObjectTextMeshProOpponentName.SetActive(true);
        gameObjectTextMeshProOpponentGreeting.SetActive(true);
        gameObjectImageOpponentAvatar.SetActive(true);

        _textMeshProOpponentName.text = userName;
        _imageOpponentAvatar.sprite = spritesAvatar[avatarId];
    }

    public void HideOpponentInformation()
    {
        gameObjectTextMeshProOpponentFound.SetActive(false);
        gameObjectTextMeshProOpponentName.SetActive(false);
        gameObjectTextMeshProOpponentGreeting.SetActive(false);
        gameObjectImageOpponentAvatar.SetActive(false);
    }
    public void ShowCountDown()
    {
        gameObjectTextMeshProCountDown.SetActive(true);
    }

    public void DrawCountDown(int count)
    {
        _textMeshProCountDown.text = "対戦開始まで・・・ " + count;
    }

    public void HideCountDown()
    {
        gameObjectTextMeshProCountDown.SetActive(false);
    }
}
