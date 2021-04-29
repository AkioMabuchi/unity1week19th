using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class GroupPrepare : SingletonMonoBehaviour<GroupPrepare>
{
    [SerializeField] private GameObject gameObjectTextMeshProQuantity;
    [SerializeField] private GameObject gameObjectTextMeshProCosts;
    [SerializeField] private GameObject gameObjectTextMeshProCost;
    [SerializeField] private GameObject gameObjectTextMeshProMaximum;
    [SerializeField] private GameObject gameObjectTextMeshProDescription;
    [SerializeField] private GameObject gameObjectImageCard;

    [SerializeField] private GameObject[] gameObjectsTextMeshProsCardName = new GameObject[7];
    [SerializeField] private GameObject[] gameObjectsTextMeshProsCardQuantity = new GameObject[7];
    [SerializeField] private GameObject[] gameObjectsButtonsIncrease = new GameObject[7];
    [SerializeField] private GameObject[] gameObjectsButtonsDecrease = new GameObject[7];

    [SerializeField] private GameObject gameObjectButtonPrev;
    [SerializeField] private GameObject gameObjectButtonNext;
    [SerializeField] private GameObject gameObjectInputFieldUserName;
    [SerializeField] private GameObject gameObjectButtonLogin;

    private TextMeshProUGUI _textMeshProQuantity;
    private TextMeshProUGUI _textMeshProCosts;
    private TextMeshProUGUI _textMeshProCost;
    private TextMeshProUGUI _textMeshProMaximum;
    private TextMeshProUGUI _textMeshProDescription;
    private Image _imageCard;

    private readonly TextMeshProUGUI[] _textMeshProsCardName = new TextMeshProUGUI[7];
    private readonly TextMeshProUGUI[] _textMeshProsCardQuantity = new TextMeshProUGUI[7];
    private readonly Button[] _buttonsIncrease = new Button[7];
    private readonly Button[] _buttonsDecrease = new Button[7];

    private Button _buttonPrev;
    private Button _buttonNext;
    private TMP_InputField _inputFieldUserName;
    private Button _buttonLogin;

    private readonly Subject<Unit> _onClickReturn = new Subject<Unit>();
    private readonly Subject<int> _onClickIncrease = new Subject<int>();
    private readonly Subject<int> _onClickDecrease = new Subject<int>();
    private readonly Subject<string> _onChangeUserName = new Subject<string>();
    private readonly Subject<Unit> _onClickLogin = new Subject<Unit>();

    public IObservable<Unit> OnClickReturn => _onClickReturn;
    public IObservable<int> OnClickIncrease => _onClickIncrease;
    public IObservable<int> OnClickDecrease => _onClickDecrease;
    public IObservable<string> OnChangeUserName => _onChangeUserName;
    public IObservable<Unit> OnClickLogin => _onClickLogin;

    private Card[] _cards;

    private int _page;

    private int[] _cardDeck = new int[100];

    private void OnEnable()
    {
        _textMeshProQuantity = gameObjectTextMeshProQuantity.GetComponent<TextMeshProUGUI>();
        _textMeshProCosts = gameObjectTextMeshProCosts.GetComponent<TextMeshProUGUI>();
        _textMeshProCost = gameObjectTextMeshProCost.GetComponent<TextMeshProUGUI>();
        _textMeshProMaximum = gameObjectTextMeshProMaximum.GetComponent<TextMeshProUGUI>();
        _textMeshProDescription = gameObjectTextMeshProDescription.GetComponent<TextMeshProUGUI>();
        _imageCard = gameObjectImageCard.GetComponent<Image>();

        for (int i = 0; i < 7; i++)
        {
            _textMeshProsCardName[i] = gameObjectsTextMeshProsCardName[i].GetComponent<TextMeshProUGUI>();
            _textMeshProsCardQuantity[i] = gameObjectsTextMeshProsCardQuantity[i].GetComponent<TextMeshProUGUI>();
            _buttonsIncrease[i] = gameObjectsButtonsIncrease[i].GetComponent<Button>();
            _buttonsDecrease[i] = gameObjectsButtonsDecrease[i].GetComponent<Button>();
        }

        _buttonPrev = gameObjectButtonPrev.GetComponent<Button>();
        _buttonNext = gameObjectButtonNext.GetComponent<Button>();
        _inputFieldUserName = gameObjectInputFieldUserName.GetComponent<TMP_InputField>();
        _buttonLogin = gameObjectButtonLogin.GetComponent<Button>();
    }

    public void OnClickButtonReturn()
    {
        _onClickReturn.OnNext(Unit.Default);
    }

    public void OnClickButtonIncrease(int index)
    {
        int cardIndex = _page * 7 + index;
        _onClickIncrease.OnNext(cardIndex);
    }

    public void OnClickButtonDecrease(int index)
    {
        int cardIndex = _page * 7 + index;
        _onClickDecrease.OnNext(cardIndex);
    }

    public void OnClickButtonPrev()
    {
        if (_page <= 0) return;
        
        _page--;
        DrawCardList();
    }

    public void OnClickButtonNext()
    {
        _page++;
        DrawCardList();
    }

    public void OnClickButtonLogin()
    {
        _onClickLogin.OnNext(Unit.Default);
    }

    public void OnPointerEnterCardName(int index)
    {
        int i = _page * 7 + index;
        if (i < _cards.Length)
        {
            string maximum = _cards[i].amount > 0 ? _cards[i].amount.ToString() : "∞"; 
            _textMeshProCost.text = "コスト: " + _cards[i].cost;
            _textMeshProMaximum.text = "最大枚数: " + maximum;
            _textMeshProDescription.text = _cards[i].description;
        }
    }

    public void OnPointerExitCardName()
    {
        _textMeshProCost.text = "コスト: -";
        _textMeshProMaximum.text = "最大枚数: -";
        _textMeshProDescription.text = "（カードネームにマウスカーソルを乗せて、カードの詳細を表示）";
    }

    public void SetCards(Card[] cards)
    {
        _cards = cards;
        DrawCardList();
    }

    void DrawCardList()
    {
        int amount = 0; // カードの総数;
        int cost = 0;

        for (int i = 0; i < _cards.Length; i++)
        {
            amount += _cardDeck[i];
            cost += _cards[i].cost * _cardDeck[i];
        }
        
        for (int i = 0; i < 7; i++)
        {
            int index = _page * 7 + i;
            if (index < _cards.Length)
            {
                _textMeshProsCardName[i].text = _cards[index].name;
                _textMeshProsCardQuantity[i].text = _cardDeck[index].ToString();
                if (_cardDeck[index] < _cards[index].amount || _cards[index].amount == 0)
                {
                    _buttonsIncrease[i].interactable = true;
                }
                else
                {
                    _buttonsIncrease[i].interactable = false;
                }
                
                if (_cardDeck[index] > 0)
                {
                    _buttonsDecrease[i].interactable = true;
                }
                else
                {
                    _buttonsDecrease[i].interactable = false;
                }
                gameObjectsTextMeshProsCardName[i].SetActive(true);
                gameObjectsTextMeshProsCardQuantity[i].SetActive(true);
                gameObjectsButtonsIncrease[i].SetActive(true);
                gameObjectsButtonsDecrease[i].SetActive(true);
            }
            else
            {
                gameObjectsTextMeshProsCardName[i].SetActive(false);
                gameObjectsTextMeshProsCardQuantity[i].SetActive(false);
                gameObjectsButtonsIncrease[i].SetActive(false);
                gameObjectsButtonsDecrease[i].SetActive(false);
            }
        }

        _textMeshProQuantity.text = "カード枚数: " + amount + " / 50";
        _textMeshProCosts.text = "カードコスト: " + cost + " / 10";

        _buttonPrev.interactable = _page > 0;
        _buttonNext.interactable = _page * 7 + 7 < _cards.Length;
    }

    public void SetCardDeck(int[] cardDeck)
    {
        _cardDeck = cardDeck;
        DrawCardList();
    }

    public void SetUserName(string userName)
    {
        _inputFieldUserName.text = userName;
    }

    public void OnDeselectInputFieldUserName()
    {
        _onChangeUserName.OnNext(_inputFieldUserName.text);
    }
}
