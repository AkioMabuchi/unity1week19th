using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

[SerializeField]
public class Cards
{
    public Card[] cards;
}

[Serializable]
public class Card
{
    public int id;
    public string name;
    public string description;
    public int cost;
    public int amount;
}

public class CardListModel : SingletonMonoBehaviour<CardListModel>
{
    [SerializeField] private string spreadsheetId;

    private readonly ReactiveProperty<Card[]> _propertyCards = new ReactiveProperty<Card[]>(new Card[0]);
    private readonly ReactiveProperty<bool> _isLoaded = new ReactiveProperty<bool>(false);
    private readonly ReactiveProperty<bool> _isError = new ReactiveProperty<bool>(false);

    public IObservable<Card[]> PropertyCards => _propertyCards;
    public IObservable<bool> IsLoaded => _isLoaded;
    public IObservable<bool> IsError => _isError;

    private IEnumerator Start()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://script.google.com/macros/s/" + spreadsheetId + "/exec");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("ネットワークエラー");
            _isError.Value = true;
        }
        else
        {
            if (request.responseCode == 200)
            {
                Debug.Log("ロード成功!");
                _propertyCards.Value = JsonUtility.FromJson<Cards>(request.downloadHandler.text).cards;
                _isLoaded.Value = true;
            }
            else
            {
                Debug.Log("エラーです");
                _isError.Value = true;
            }
        }
    }
}
