using System;
using UnityEngine;
using UniRx;

public class CardDeckModel :SingletonMonoBehaviour<CardDeckModel>
{
    [SerializeField] private string cardDeckKey;

    private readonly ReactiveProperty<int[]> _cardDeck = new ReactiveProperty<int[]>();
    public IObservable<int[]> CardDeck => _cardDeck;

    private readonly int[][] _defaultCardDeck =
    {
        new[]
        {
            34, 0, 0, 0, 0, 0, 0, 4, 5, 1, 1, 3, 1, 0, 1, 0, 0, 0
        },
        new[]
        {
            21, 11, 0, 0, 7, 0, 0, 3, 1, 0, 1, 1, 0, 1, 1, 3, 0, 0
        },
        new[]
        {
            18, 0, 13, 0, 0, 8, 0, 2, 0, 1, 1, 2, 1, 0, 2, 0, 2, 0
        },
        new[]
        {
            25, 0, 0, 9, 0, 0, 5, 3, 2, 1, 1, 1, 0, 1, 1, 0, 0, 1
        }
    };
    private void Start()
    {
        if (ES3.KeyExists(cardDeckKey))
        {
            int[] cardDeck = ES3.Load<int[]>(cardDeckKey);
            _cardDeck.SetValueAndForceNotify(cardDeck);
        }
        else
        {
            int[] cardDeck = new int[100];
            for (int i = 0; i < 100; i++)
            {
                cardDeck[i] = 0;
            }
            _cardDeck.SetValueAndForceNotify(cardDeck);
        }

        _cardDeck.Subscribe(cardDeck =>
        {
            ES3.Save(cardDeckKey, cardDeck);
        });
    }

    public void IncreaseCard(int cardIndex)
    {
        int[] cardDeck = _cardDeck.Value;
        if (cardDeck[cardIndex] < 99)
        {
            cardDeck[cardIndex]++;
        }
        _cardDeck.SetValueAndForceNotify(cardDeck);
    }

    public void DecreaseCard(int cardIndex)
    {
        int[] cardDeck = _cardDeck.Value;
        if (cardDeck[cardIndex] > 0)
        {
            cardDeck[cardIndex]--;
        }
        _cardDeck.SetValueAndForceNotify(cardDeck);
    }

    public void SetDefaultDeck()
    {
        int index = UnityEngine.Random.Range(0, _defaultCardDeck.Length);
        _cardDeck.SetValueAndForceNotify(_defaultCardDeck[index]);
    }
}
