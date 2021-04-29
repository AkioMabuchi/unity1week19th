using System;
using UnityEngine;
using UniRx;

public class CardDeckModel :SingletonMonoBehaviour<CardDeckModel>
{
    [SerializeField] private string cardDeckKey;

    private readonly ReactiveProperty<int[]> _cardDeck = new ReactiveProperty<int[]>();
    public IObservable<int[]> CardDeck => _cardDeck;

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
}
