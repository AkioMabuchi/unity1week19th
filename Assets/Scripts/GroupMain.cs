using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GroupMain : SingletonMonoBehaviour<GroupMain>
{
    [SerializeField] private Sprite spriteCardDummy;
    [SerializeField] private Sprite spriteCardBack;
    [SerializeField] private Sprite[] spriteCards;
    
    [SerializeField] private GameObject gameObjectTextMeshProPlayerUserName;
    [SerializeField] private GameObject gameObjectTextMeshProPlayerHp;
    [SerializeField] private GameObject gameObjectTextMeshProPlayerSword;
    [SerializeField] private GameObject gameObjectTextMeshProPlayerCondition;
    [SerializeField] private GameObject gameObjectTextMeshProPlayerCards;

    [SerializeField] private GameObject gameObjectTextMeshProOpponentUserName;
    [SerializeField] private GameObject gameObjectTextMeshProOpponentHp;
    [SerializeField] private GameObject gameObjectTextMeshProOpponentSword;
    [SerializeField] private GameObject gameObjectTextMeshProOpponentCondition;
    [SerializeField] private GameObject gameObjectTextMeshProOpponentCards;
    
    [SerializeField] private GameObject[] gameObjectsImagesPlayerCard = new GameObject[6];
    [SerializeField] private GameObject[] gameObjectsImagesOpponentCard = new GameObject[6];

    [SerializeField] private GameObject gameObjectTextMeshProDiscardNotice;
    [SerializeField] private GameObject gameObjectTextMeshProDiscardTimer;
    
    [SerializeField] private GameObject gameObjectImageAnnounce;
    [SerializeField] private GameObject[] gameObjectsTextMeshProsAnnounce = new GameObject[10];

    [SerializeField] private GameObject gameObjectImageCall;
    [SerializeField] private GameObject gameObjectTextMeshProCall;
    [SerializeField] private GameObject gameObjectImageCallSmall;
    [SerializeField] private GameObject gameObjectTextMeshProCallSmall;

    private readonly TextMeshProUGUI[] _textMeshProsFighterUserName = new TextMeshProUGUI[2];
    private readonly TextMeshProUGUI[] _textMeshProsFighterHp = new TextMeshProUGUI[2];
    private readonly TextMeshProUGUI[] _textMeshProsFighterSword = new TextMeshProUGUI[2];
    private readonly TextMeshProUGUI[] _textMeshProsFighterCondition = new TextMeshProUGUI[2];
    private readonly TextMeshProUGUI[] _textMeshProsFighterCards = new TextMeshProUGUI[2];
    
    
    private readonly Image[] _imagesPlayerCard = new Image[6];
    private readonly Image[] _imagesOpponentCard = new Image[6];

    private TextMeshProUGUI _textMeshProDiscardNotice;
    private TextMeshProUGUI _textMeshProDiscardTimer;
    
    private TextMeshProUGUI _textMeshProCall;
    private TextMeshProUGUI _textMeshProCallSmall;
    
    private readonly TextMeshProUGUI[] _textMeshProsAnnounce = new TextMeshProUGUI[10];
    private readonly Subject<Unit> _onEndDiscard = new Subject<Unit>();
    private readonly Subject<Unit> _onEndCallBanner = new Subject<Unit>();
    private readonly Subject<Unit> _onEndCallSmallBanner = new Subject<Unit>();
    private readonly Subject<int> _onSelectCard = new Subject<int>();
    private readonly Subject<Unit> _onDeselectCard = new Subject<Unit>();
    private readonly Subject<Unit> _onClickCard = new Subject<Unit>();
    public IObservable<Unit> OnEndDiscard => _onEndDiscard;
    public IObservable<Unit> OnEndCallBanner => _onEndCallBanner;
    public IObservable<Unit> OnEndCallSmallBanner => _onEndCallSmallBanner;
    public IObservable<int> OnSelectCard => _onSelectCard;
    public IObservable<Unit> OnDeselectCard => _onDeselectCard;
    public IObservable<Unit> OnClickCard => _onClickCard;

    private int _announceIndex;
    private void OnEnable()
    {
        _textMeshProsFighterUserName[0] = gameObjectTextMeshProPlayerUserName.GetComponent<TextMeshProUGUI>();
        _textMeshProsFighterHp[0] = gameObjectTextMeshProPlayerHp.GetComponent<TextMeshProUGUI>();
        _textMeshProsFighterSword[0] = gameObjectTextMeshProPlayerSword.GetComponent<TextMeshProUGUI>();
        _textMeshProsFighterCondition[0] = gameObjectTextMeshProPlayerCondition.GetComponent<TextMeshProUGUI>();
        _textMeshProsFighterCards[0] = gameObjectTextMeshProPlayerCards.GetComponent<TextMeshProUGUI>();
            
        _textMeshProsFighterUserName[1] = gameObjectTextMeshProOpponentUserName.GetComponent<TextMeshProUGUI>();
        _textMeshProsFighterHp[1] = gameObjectTextMeshProOpponentHp.GetComponent<TextMeshProUGUI>();
        _textMeshProsFighterSword[1] = gameObjectTextMeshProOpponentSword.GetComponent<TextMeshProUGUI>();
        _textMeshProsFighterCondition[1] = gameObjectTextMeshProOpponentCondition.GetComponent<TextMeshProUGUI>();
        _textMeshProsFighterCards[1] = gameObjectTextMeshProOpponentCards.GetComponent<TextMeshProUGUI>();
        
        for (int i = 0; i < 6; i++)
        {
            _imagesPlayerCard[i] = gameObjectsImagesPlayerCard[i].GetComponent<Image>();
            _imagesOpponentCard[i] = gameObjectsImagesOpponentCard[i].GetComponent<Image>();
        }

        _textMeshProDiscardNotice = gameObjectTextMeshProDiscardNotice.GetComponent<TextMeshProUGUI>();
        _textMeshProDiscardTimer = gameObjectTextMeshProDiscardTimer.GetComponent<TextMeshProUGUI>();
        
        _textMeshProCall = gameObjectTextMeshProCall.GetComponent<TextMeshProUGUI>();
        _textMeshProCallSmall = gameObjectTextMeshProCallSmall.GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < 10; i++)
        {
            _textMeshProsAnnounce[i] = gameObjectsTextMeshProsAnnounce[i].GetComponent<TextMeshProUGUI>();
        }
    }

    public void DrawFighterUserName(string userName, int index)
    {
        _textMeshProsFighterUserName[index].text = userName;
    }

    public void UpdateFighterHp(int hp, int index)
    {
        _textMeshProsFighterHp[index].text = "HP: " + hp;
    }

    public void UpdateFighterSword(FighterSword sword, int index)
    {
        string textSword = "";
        switch (sword)
        {
            case FighterSword.Normal:
                textSword = "<color=#7F7F7F>無属性</color>";
                break;
            case FighterSword.Fire:
                textSword = "<color=#F2A561>炎属性</color>";
                break;
            case FighterSword.Thunder:
                textSword = "<color=#F2E555>雷属性</color>";
                break;
            case FighterSword.Ice:
                textSword = "<color=#95D8E5>氷属性</color>";
                break;
        }
        
        _textMeshProsFighterSword[index].text = "剣: " + textSword;
    }

    public void UpdateFighterCondition(FighterCondition condition, int index)
    {
        string textCondition = "";
        switch (condition)
        {
            case FighterCondition.Fine:
                textCondition = "正常";
                break;
            case FighterCondition.Burned:
                textCondition = "<color=#F2A561>火傷</color>";
                break;
            case FighterCondition.Paralysis:
                textCondition = "<color=#F2E555>麻痺</color>";
                break;
            case FighterCondition.Frozen:
                textCondition = "<color=#95D8E5>凍結</color>";
                break;
        }
        
        _textMeshProsFighterCondition[index].text = "状態: " + textCondition;
    }

    public void UpdateFighterCardQuantity(int quantity, int index)
    {
        _textMeshProsFighterCards[index].text = "カード: " + quantity;
    }

    public void DrawPlayerCards(int[] cardHand, int selectedCard1, int selectedCard2)
    {
        float[] positionsX = {-52.0f, -14.0f, 24.0f, 62.0f, 100.0f, 138.0f};
        float[] positionsY = {-60.0f, -60.0f, -60.0f, -60.0f, -60.0f, -60.0f};
        int[] selectedCards = {selectedCard1, selectedCard2};
        for (int i = 0; i < 6; i++)
        {
            if (cardHand[i] >= 0)
            {
                gameObjectsImagesPlayerCard[i].SetActive(true);
                _imagesPlayerCard[i].sprite = spriteCards[cardHand[i]];
            }
            else
            {
                gameObjectsImagesPlayerCard[i].SetActive(false);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            if (selectedCards[i] >= 0)
            {
                positionsY[selectedCards[i]] = -50.0f;
            }
        }

        for (int i = 0; i < 6; i++)
        {
            gameObjectsImagesPlayerCard[i].transform.localPosition = new Vector3(positionsX[i], positionsY[i], 0.0f);
        }
    }

    public void DrawOpponentCards(int[] cardHand)
    {
        float[] positionsX = {-52.0f, -14.0f, 24.0f, 62.0f, 100.0f, 138.0f};
        for (int i = 0; i < 6; i++)
        {
            if (cardHand[i] >= 0)
            {
                gameObjectsImagesOpponentCard[i].SetActive(true);
                _imagesOpponentCard[i].sprite = i > 2 ? spriteCards[cardHand[i]] : spriteCardBack;
            }
            else
            {
                gameObjectsImagesOpponentCard[i].SetActive(false);
            }
        }

        for (int i = 0; i < 6; i++)
        {
            gameObjectsImagesOpponentCard[i].transform.localPosition = new Vector3(positionsX[i],60.0f, 0.0f);
        }
    }

    public void OnPointerEnterCard(int index)
    {
        _onSelectCard.OnNext(index);
    }

    public void OnPointerExitCard()
    {
        _onDeselectCard.OnNext(Unit.Default);
    }

    public void OnPointerDownCard()
    {
        _onClickCard.OnNext(Unit.Default);
    }
    
    public void ShowDiscardNotice(bool hasCard, FighterCondition condition)
    {
        string text;
        if (hasCard)
        {
            switch (condition)
            {
                case FighterCondition.Burned:
                    text = "<color=#F2A561>火傷で攻撃力が1低下しています</color>";
                    break;
                case FighterCondition.Paralysis:
                    text = "<color=#F2E555>麻痺で1枚だけカードを選択できます</color>";
                    break;
                case FighterCondition.Frozen:
                    text = "<color=#95D8E5>凍結でカードを出せません</color>";
                    break;
                default:
                    text = "カードを1-2枚、選択してください";
                    break;
            }
        }
        else
        {
            text = "<color=#7F7F7F>出せるカードがありません</color>";
        }

        _textMeshProDiscardNotice.text = text;
        gameObjectTextMeshProDiscardNotice.SetActive(true);
    }

    public void HideDiscardNotice()
    {
        gameObjectTextMeshProDiscardNotice.SetActive(false);
    }

    public void ShowDiscardTimer()
    {
        gameObjectTextMeshProDiscardTimer.SetActive(true);
    }

    public void DrawDiscardTimer(int count)
    {
        string num1 = (count / 10).ToString();
        string num2 = (count % 10).ToString();

        _textMeshProDiscardTimer.text = num1 + "." + num2 + "\"";
    }

    public void HideDiscardTimer()
    {
        gameObjectTextMeshProDiscardTimer.SetActive(false);
    }

    public void Discard(int playerSelect1, int playerSelect2, int opponentSelect1, int opponentSelect2,
        int[] opponentCardHand)
    {
        if (playerSelect1 >= 0)
        {
            gameObjectsImagesPlayerCard[playerSelect1].transform.DOLocalMove(new Vector3(81.0f, 0.0f, 0.0f), 0.6f)
                .SetEase(Ease.OutQuad);
        }

        if (playerSelect2 >= 0)
        {
            gameObjectsImagesPlayerCard[playerSelect2].transform.DOLocalMove(new Vector3(119.0f, 0.0f, 0.0f), 0.6f)
                .SetEase(Ease.OutQuad);
        }

        if (opponentSelect1 >= 0)
        {
            gameObjectsImagesOpponentCard[opponentSelect1].transform.DOLocalMove(new Vector3(5.0f, 0.0f, 0.0f), 0.6f)
                .SetEase(Ease.OutQuad);
            _imagesOpponentCard[opponentSelect1].sprite = spriteCards[opponentCardHand[opponentSelect1]];
        }
        
        if (opponentSelect2 >= 0)
        {
            gameObjectsImagesOpponentCard[opponentSelect2].transform.DOLocalMove(new Vector3(-33.0f, 0.0f, 0.0f), 0.6f)
                .SetEase(Ease.OutQuad);
            _imagesOpponentCard[opponentSelect2].sprite = spriteCards[opponentCardHand[opponentSelect2]];
        }

        Observable.Timer(TimeSpan.FromSeconds(2.0f)).Subscribe(_ =>
        {
            _onEndDiscard.OnNext(Unit.Default);
        });
    }

    public void ClearFieldCards(int playerSelect1, int playerSelect2, int opponentSelect1, int opponentSelect2)
    {
        if (playerSelect1 >= 0)
        {
            _imagesPlayerCard[playerSelect1].sprite = spriteCardDummy;
        }

        if (playerSelect2 >= 0)
        {
            _imagesPlayerCard[playerSelect2].sprite = spriteCardDummy;
        }

        if (opponentSelect1 >= 0)
        {
            _imagesOpponentCard[opponentSelect1].sprite = spriteCardDummy;
        }

        if (opponentSelect2 >= 0)
        {
            _imagesOpponentCard[opponentSelect2].sprite = spriteCardDummy;
        }
    }

    public void ShowCallBanner(string text)
    {
        _textMeshProCall.text = text;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(gameObjectImageCall.transform.DOScaleY(1.0f, 0.5f).SetEase(Ease.OutQuad));
        sequence.Append(gameObjectTextMeshProCall.transform.DOLocalMoveX(0.0f, 0.8f).SetEase(Ease.OutQuad));
        sequence.AppendInterval(1.5f);
        sequence.Append(gameObjectTextMeshProCall.transform.DOLocalMoveX(-320.0f, 0.8f).SetEase(Ease.InQuad));
        sequence.Append(gameObjectImageCall.transform.DOScaleY(0.0f, 0.5f).SetEase(Ease.InQuad));
        sequence.AppendInterval(1.0f);
        sequence.OnComplete(() =>
        {
            gameObjectImageCall.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
            gameObjectTextMeshProCall.transform.localPosition = new Vector3(320.0f, 0.0f, 0.0f);
            
            gameObjectImageCall.SetActive(false);
            gameObjectTextMeshProCall.SetActive(false);
            
            _onEndCallBanner.OnNext(Unit.Default);
        });
        gameObjectImageCall.SetActive(true);
        gameObjectTextMeshProCall.SetActive(true);
    }
    
    public void ShowCallSmallBanner(string text)
    {
        _textMeshProCallSmall.text = text;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(gameObjectImageCallSmall.transform.DOScaleY(1.0f, 0.5f).SetEase(Ease.OutQuad));
        sequence.Append(gameObjectTextMeshProCallSmall.transform.DOLocalMoveX(0.0f, 0.8f).SetEase(Ease.OutQuad));
        sequence.AppendInterval(1.5f);
        sequence.Append(gameObjectTextMeshProCallSmall.transform.DOLocalMoveX(-320.0f, 0.8f).SetEase(Ease.InQuad));
        sequence.Append(gameObjectImageCallSmall.transform.DOScaleY(0.0f, 0.5f).SetEase(Ease.InQuad));
        sequence.AppendInterval(1.0f);
        sequence.OnComplete(() =>
        {
            gameObjectImageCallSmall.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
            gameObjectTextMeshProCallSmall.transform.localPosition = new Vector3(320.0f, 0.0f, 0.0f);
            
            gameObjectImageCallSmall.SetActive(false);
            gameObjectTextMeshProCallSmall.SetActive(false);
            
            _onEndCallSmallBanner.OnNext(Unit.Default);
        });
        gameObjectImageCallSmall.SetActive(true);
        gameObjectTextMeshProCallSmall.SetActive(true);
    }

    public void InitializeAnnounceBoard()
    {
        for (int i = 0; i < 10; i++)
        {
            _textMeshProsAnnounce[i].text = "";
        }

        _announceIndex = 0;
    }

    public void ShowAnnounceBoard()
    {
        gameObjectImageAnnounce.SetActive(true);
    }

    public void DrawAnnounceBoard(string text)
    {
        if (_announceIndex < 10)
        {
            _textMeshProsAnnounce[_announceIndex].text = text;
            _announceIndex++;
        }
        else
        {
            for (int i = 1; i < 10; i++)
            {
                _textMeshProsAnnounce[i - 1].text = _textMeshProsAnnounce[i].text;
            }

            _textMeshProsAnnounce[10].text = text;
        }
    }

    public void HideAnnounceBoard()
    {
        gameObjectImageAnnounce.SetActive(false);
    }
}
