using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UniRx;

[Serializable]
public class FighterInformation
{
    public int[] cardHand;
    public int hp;
    public FighterSword sword;
    public FighterCondition condition;
    public int cardQuantity;
    public int selectedCard1;
    public int selectedCard2;
    public int readyCode;
} 
public enum FighterSword
{
    Normal,
    Fire,
    Thunder,
    Ice
}

public enum FighterCondition
{
    Fine,
    Burned,
    Paralysis,
    Frozen
}
public interface IFighter
{
    public void Initialize(string userName, int avatarId, int[] rawCardDeck);
    public bool IsPlayer();
    public bool IsOpponent();

    public string UserName
    {
        get;
    }
    public int AvatarId
    {
        get;
    }

    public FighterInformation GetInformation();
    public void DrawCard(int index);
    public void DrawCards();
    public void UpdateHands();
    public void SetHp(int hp);
    public void SetSword(FighterSword sword);
    public void SetCondition(FighterCondition condition);
    public void SelectCard(int index);
    public void SetReadyCode(int readyCode);
}
public class Fighter : MonoBehaviourPunCallbacks, IPunObservable, IFighter
{
    private PhotonView _photonView;

    private string _userName;
    private int _avatarId;
    private int[] _cardDeck = new int[0];
    private int _cardDeckIndex = 0;
    private int[] _cardHand = {-1, -1, -1, -1, -1, -1};
    private int _hp = 10;
    private FighterSword _sword = FighterSword.Normal;
    private FighterCondition _condition = FighterCondition.Fine;
    private int _selectedCard1 = -1;
    private int _selectedCard2 = -1;
    private int _readyCode = 0;

    public string UserName => _userName;
    public int AvatarId => _avatarId;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_userName);
            stream.SendNext(_avatarId);
            stream.SendNext(_cardDeck);
            stream.SendNext(_cardDeckIndex);
            stream.SendNext(_cardHand);
            stream.SendNext(_hp);
            stream.SendNext(_sword);
            stream.SendNext(_condition);
            stream.SendNext(_selectedCard1);
            stream.SendNext(_selectedCard2);
            stream.SendNext(_readyCode);
        }
        else
        {
            _userName = (string) stream.ReceiveNext();
            _avatarId = (int) stream.ReceiveNext();
            _cardDeck = (int[]) stream.ReceiveNext();
            _cardDeckIndex = (int) stream.ReceiveNext();
            _cardHand = (int[]) stream.ReceiveNext();
            _hp = (int) stream.ReceiveNext();
            _sword = (FighterSword) stream.ReceiveNext();
            _condition = (FighterCondition) stream.ReceiveNext();
            _selectedCard1 = (int) stream.ReceiveNext();
            _selectedCard2 = (int) stream.ReceiveNext();
            _readyCode = (int) stream.ReceiveNext();
        }
    }

    public void Initialize(string userName, int avatarId, int[] rawCardDeck)
    {
        _userName = userName;
        _avatarId = avatarId;
        
        List<int> cardDeck = new List<int>();
        for (int i = 0; i < rawCardDeck.Length; i++)
        {
            for (int j = 0; j < rawCardDeck[i]; j++)
            {
                cardDeck.Add(i);
            }
        }

        int k = cardDeck.Count - 1;
        while (k > 0)
        {
            int index = UnityEngine.Random.Range(0, k);
            int tmp = cardDeck[k];
            cardDeck[k] = cardDeck[index];
            cardDeck[index] = tmp;
            k--;
        }

        _cardDeck = cardDeck.ToArray();
    }

    public bool IsPlayer()
    {
        return _photonView.IsMine;
    }

    public bool IsOpponent()
    {
        return !_photonView.IsMine;
    }

    public FighterInformation GetInformation()
    {
        return new FighterInformation
        {
            cardHand = _cardHand,
            hp = _hp,
            sword = _sword,
            condition = _condition,
            cardQuantity = _cardDeck.Length - _cardDeckIndex,
            selectedCard1 = _selectedCard1,
            selectedCard2 = _selectedCard2,
            readyCode =  _readyCode
        };
    }

    public void DrawCard(int index)
    {
        if (_cardHand[index] < 0 && _cardDeckIndex < _cardDeck.Length)
        {
            _cardHand[index] = _cardDeck[_cardDeckIndex];
            _cardDeckIndex++;
        }
    }

    public void DrawCards()
    {
        for (int i = 0; i < 6; i++)
        {
            if (_cardHand[i] < 0 && _cardDeckIndex < _cardDeck.Length)
            {
                _cardHand[i] = _cardDeck[_cardDeckIndex];
                _cardDeckIndex++;
            }
        }
    }

    public void UpdateHands()
    {
        if (_selectedCard1 >= 0)
        {
            _cardHand[_selectedCard1] = -1;
        }

        if (_selectedCard2 >= 0)
        {
            _cardHand[_selectedCard2] = -1;
        }

        _selectedCard1 = -1;
        _selectedCard2 = -1;
    }

    public void SetHp(int hp)
    {
        _hp = hp;
    }

    public void SetSword(FighterSword sword)
    {
        _sword = sword;
    }

    public void SetCondition(FighterCondition condition)
    {
        _condition = condition;
    }

    public void SelectCard(int index)
    {
        switch (_condition)
        {
            case FighterCondition.Paralysis:
                if (_selectedCard1 == index)
                {
                    _selectedCard1 = -1;
                }
                else
                {
                    _selectedCard1 = index;
                }
                _selectedCard2 = -1;
                break;
            case FighterCondition.Frozen:
                break;
            default:
                if (_selectedCard1 == index)
                {
                    if (_selectedCard2 >= 0)
                    {
                        _selectedCard1 = _selectedCard2;
                        _selectedCard2 = -1;
                    }
                    else
                    {
                        _selectedCard1 = -1;
                    }
                }
                else if (_selectedCard2 == index)
                {
                    _selectedCard2 = -1;
                }
                else if (_selectedCard1 >= 0)
                {
                    _selectedCard2 = index;
                }
                else
                {
                    _selectedCard1 = index;
                }
                break;
        }
    }

    public void SetReadyCode(int readyCode)
    {
        _readyCode = readyCode;
    }
}
