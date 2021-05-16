using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PrepareSceneManager : PresenterMonoBehaviour
{
    private void Start()
    {
        Disposables.Add(CardListModel.Instance.PropertyCards.Subscribe(cards =>
        {
            GroupPrepare.Instance.SetCards(cards);
        }));
        
        Disposables.Add(CardDeckModel.Instance.CardDeck.Subscribe(cardDeck =>
        {
            GroupPrepare.Instance.SetCardDeck(cardDeck);
        }));

        Disposables.Add(GroupPrepare.Instance.OnClickReturn.Subscribe(_ =>
        {
            SoundPlayer.Instance.PlaySound(2);
            SceneController.Instance.ChangeScene("PrepareScene", "TitleScene");
        }));

        Disposables.Add(GroupPrepare.Instance.OnClickIncrease.Subscribe(cardIndex =>
        {
            CardDeckModel.Instance.IncreaseCard(cardIndex);
        }));

        Disposables.Add(GroupPrepare.Instance.OnClickDecrease.Subscribe(cardIndex =>
        {
            CardDeckModel.Instance.DecreaseCard(cardIndex);
        }));
        
        Disposables.Add(UserNameModel.Instance.UserName.Subscribe(userName =>
        {
            GroupPrepare.Instance.SetUserName(userName);
        }));
        
        Disposables.Add(GroupPrepare.Instance.OnChangeUserName.Subscribe(userName =>
        {
            UserNameModel.Instance.SetUserName(userName);
        }));
        
        Disposables.Add(GroupPrepare.Instance.OnClickLogin.Subscribe(_ =>
        {
            SoundPlayer.Instance.PlaySound(1);
            SceneController.Instance.ChangeScene("PrepareScene", "MainScene");
        }));
        
        Disposables.Add(GroupPrepare.Instance.OnClickDefaultDeck.Subscribe(_ =>
        {
            CardDeckModel.Instance.SetDefaultDeck();
        }));
    }
}
