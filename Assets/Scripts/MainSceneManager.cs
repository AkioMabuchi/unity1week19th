using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public enum MainScenePhrase
{
    Idle,
    Introduction,
    StartCall,
    Card,
    Discard,
    Action,
    Next,
    Finish
}

// 注意、このクラスは非常に行数が長くなる可能性がある。
public class MainSceneManager : PresenterMonoBehaviourPunCallbacks
{
    private bool _isConnected;
    private bool _isMatched;
    private bool _isInformationUpdating;
    private bool _isSelectable;

    private readonly List<Coroutine> _coroutines = new List<Coroutine>();
    
    private IFighter _fighterPlayer;
    private IFighter _fighterOpponent;

    private MainScenePhrase _phrase = MainScenePhrase.Idle;
    
    private int[] _rawCardDeck = new int[0];
    
    private string _playerUserName;
    private int _playerAvatarId = 0;
    private int[] _playerCardHand = new int[6];
    private int _playerHp = 10;
    private FighterSword _playerSword = FighterSword.Normal;
    private FighterCondition _playerCondition = FighterCondition.Fine;
    private int _playerCardQuantity = 0;
    private int _playerSelectedCard1 = -1;
    private int _playerSelectedCard2 = -1;
    private bool _hasPlayerCard = true;
    private int _playerReadyCode = 0;
    
    private string _opponentUserName;
    private int _opponentAvatarId;
    private int[] _opponentCardHand = new int[6];
    private int _opponentHp = 10;
    private FighterSword _opponentSword = FighterSword.Normal;
    private FighterCondition _opponentCondition = FighterCondition.Fine;
    private int _opponentCardQuantity = 0;
    private int _opponentSelectedCard1 = -1;
    private int _opponentSelectedCard2 = -1;
    private bool _hasOpponentCard = true;
    private int _opponentReadyCode = 0;

    private int _selectedCard = -1;
    
    private bool _hasOpponentRun = false;
    
    

    private void Start()
    {
        Disposables.Add(GroupConnection.Instance.OnClickReturn.Subscribe(_ =>
        {
            SoundPlayer.Instance.PlaySound(2);
            PhotonNetwork.Disconnect();
            SceneController.Instance.ChangeScene("MainScene", "PrepareScene");
        }));
        
        Disposables.Add(GroupConnection.Instance.OnClickReconnection.Subscribe(_ =>
        {
            _coroutines.Add(StartCoroutine(CoroutineServerConnection()));
        }));
        
        Disposables.Add(UserNameModel.Instance.UserName.Subscribe(userName =>
        {
            _playerUserName = userName;
        }));
        
        Disposables.Add(AvatarModel.Instance.AvatarId.Subscribe(id =>
        {
            _playerAvatarId = id;
        }));
        
        Disposables.Add(CardDeckModel.Instance.CardDeck.Subscribe(rawCardDeck =>
        {
            _rawCardDeck = rawCardDeck;
        }));
        
        Disposables.Add(GroupSceneEffect.Instance.OnCompleteHide.Subscribe(_ =>
        {
            GroupConnection.Instance.HideView();
            GroupMain.Instance.DrawFighterUserName(_playerUserName, 0);
            GroupMain.Instance.DrawFighterUserName(_opponentUserName, 1);
            TitleMusicPlayer.Instance.StopMusic();
            
            Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe(__ =>
            {
                GroupSceneEffect.Instance.Show();
            });
        }));
        
        Disposables.Add(GroupSceneEffect.Instance.OnCompleteShow.Subscribe(_ =>
        {                
            MainMusicPlayer.Instance.StartMusic();
            _isInformationUpdating = true;
            _phrase = MainScenePhrase.Introduction;
            _coroutines.Add(StartCoroutine(CoroutineStartHands()));
        }));
        
        Disposables.Add(GroupMain.Instance.OnEndDiscard.Subscribe(_ =>
        {
            if (_phrase == MainScenePhrase.Discard)
            {
                _fighterPlayer?.SetReadyCode(4);
            }
        }));
        Disposables.Add(GroupMain.Instance.OnEndCallBanner.Subscribe(_ =>
        {
            if (_phrase == MainScenePhrase.StartCall)
            {
                _fighterPlayer?.SetReadyCode(2);
            }

            if (_phrase == MainScenePhrase.Finish)
            {
                Observable.Timer(TimeSpan.FromSeconds(1.0f)).Subscribe(__ =>
                {
                    if (_playerHp > _opponentHp)
                    {
                        ResultModel.Instance.SetResult(GameResult.Win);
                    }
                    
                    if (_playerHp == _opponentHp)
                    {
                        ResultModel.Instance.SetResult(GameResult.Draw);
                    }

                    if (_playerHp < _opponentHp)
                    {
                        ResultModel.Instance.SetResult(GameResult.Lose);
                    }
                    GroupFadeEffect.Instance.FadeIn();
                });
            }
        }));
        
        Disposables.Add(GroupMain.Instance.OnEndCallSmallBanner.Subscribe(_ =>
        {
            if (_phrase == MainScenePhrase.Next)
            {
                _fighterPlayer?.SetReadyCode(6);
            }
        }));
        
        Disposables.Add(GroupMain.Instance.OnSelectCard.Subscribe(index =>
        {
            if (_isSelectable)
            {
                _selectedCard = index;
            }
        }));
        
        Disposables.Add(GroupMain.Instance.OnDeselectCard.Subscribe(_ =>
        {
            _selectedCard = -1;
        }));
        
        Disposables.Add(GroupMain.Instance.OnClickCard.Subscribe(_ =>
        {
            if (_selectedCard >= 0)
            {
                _fighterPlayer?.SelectCard(_selectedCard);
            }
        }));
        
        Disposables.Add(GroupFadeEffect.Instance.OnEndFadeIn.Subscribe(_ =>
        {
            MainMusicPlayer.Instance.StopMusic();
            PhotonNetwork.Disconnect();
            SceneController.Instance.ChangeScene("MainScene", "ResultScene");
        }));
        
        PhotonNetwork.GameVersion = "Version 1.0.0";

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                switch (_phrase)
                {
                    case MainScenePhrase.Introduction:
                        if (_playerReadyCode == 1 && _opponentReadyCode == 1)
                        {
                            _phrase = MainScenePhrase.StartCall;
                            SoundPlayer.Instance.PlaySound(6);
                            GroupMain.Instance.ShowCallBanner("対戦開始!");
                        }
                        break;
                    case MainScenePhrase.StartCall:
                        if (_playerReadyCode == 2 && _opponentReadyCode == 2)
                        {
                            _phrase = MainScenePhrase.Card;
                            _coroutines.Add(StartCoroutine(CoroutineCountDownCard()));
                            _isSelectable = true;
                            GroupMain.Instance.ShowDiscardNotice(_hasPlayerCard, _playerCondition);
                            GroupMain.Instance.ShowDiscardTimer();
                        }
                        break;
                    case MainScenePhrase.Card:
                        if (_playerReadyCode == 3 && _opponentReadyCode == 3)
                        {
                            _phrase = MainScenePhrase.Discard;
                            _isInformationUpdating = false;
                            SoundPlayer.Instance.PlaySound(5);
                            GroupMain.Instance.Discard(_playerSelectedCard1, _playerSelectedCard2, _opponentSelectedCard1,
                                _opponentSelectedCard2, _opponentCardHand);
                            GroupMain.Instance.HideDiscardTimer();
                        }

                        break;
                    case MainScenePhrase.Discard:
                        if (_playerReadyCode == 4 && _opponentReadyCode == 4)
                        {
                            _phrase = MainScenePhrase.Action;
                            
                            _coroutines.Add(StartCoroutine(CoroutineProcessActionQueries()));
                        }

                        break;
                    case MainScenePhrase.Action:
                        if (_playerReadyCode == 5 && _opponentReadyCode == 5)
                        {
                            if (_playerHp > 0 && _opponentHp > 0 && (_hasPlayerCard || _hasOpponentCard))
                            {
                                _isInformationUpdating = true;
                                _phrase = MainScenePhrase.Next;
                                _fighterPlayer?.UpdateHands();
                                _fighterPlayer?.DrawCards();
                                GroupMain.Instance.ShowCallSmallBanner("次のターン");
                            }
                            else
                            {
                                _isInformationUpdating = true;
                                _phrase = MainScenePhrase.Finish;
                                _isMatched = false;
                                _fighterPlayer?.UpdateHands();
                                SoundPlayer.Instance.PlaySound(7);
                                GroupMain.Instance.ShowCallBanner("そこまで!");
                            }
                        }

                        break;
                    case MainScenePhrase.Next:
                        if (_playerReadyCode == 6 && _opponentReadyCode == 6)
                        {
                            _phrase = MainScenePhrase.Card;
                            _coroutines.Add(StartCoroutine(CoroutineCountDownCard()));
                            _isSelectable = true;
                            GroupMain.Instance.ShowDiscardNotice(_hasPlayerCard, _playerCondition);
                            GroupMain.Instance.ShowDiscardTimer();
                        }
                        break;
                }
            });
        
        
        this.UpdateAsObservable()
            .Where(_ => _fighterPlayer != null)
            .Subscribe(_ =>
            {
                FighterInformation information = _fighterPlayer.GetInformation();
                _playerCardHand = information.cardHand;
                _playerHp = information.hp;
                _playerSword = information.sword;
                _playerCondition = information.condition;
                _playerCardQuantity = information.cardQuantity;
                _playerSelectedCard1 = information.selectedCard1;
                _playerSelectedCard2 = information.selectedCard2;

                bool hasCard = false;
                for (int i = 0; i < 6; i++)
                {
                    if (_playerCardHand[i] >= 0)
                    {
                        hasCard = true;
                    }
                }

                _hasPlayerCard = hasCard;

                _playerReadyCode = information.readyCode;
            });

        this.UpdateAsObservable()
            .Where(_ => _fighterOpponent != null)
            .Subscribe(_ =>
            {
                FighterInformation information = _fighterOpponent.GetInformation();
                _opponentCardHand = information.cardHand;
                _opponentHp = information.hp;
                _opponentSword = information.sword;
                _opponentCondition = information.condition;
                _opponentCardQuantity = information.cardQuantity;
                _opponentSelectedCard1 = information.selectedCard1;
                _opponentSelectedCard2 = information.selectedCard2;
                
                bool hasCard = false;
                for (int i = 0; i < 6; i++)
                {
                    if (_opponentCardHand[i] >= 0)
                    {
                        hasCard = true;
                    }
                }

                _hasOpponentCard = hasCard;

                _opponentReadyCode = information.readyCode;
            });

        this.UpdateAsObservable()
            .Where(_ => _isInformationUpdating)
            .Subscribe(_ =>
            {
                GroupMain.Instance.UpdateFighterHp(_playerHp, 0);
                GroupMain.Instance.UpdateFighterSword(_playerSword, 0);
                GroupMain.Instance.UpdateFighterCondition(_playerCondition, 0);
                GroupMain.Instance.UpdateFighterCardQuantity(_playerCardQuantity, 0);
                GroupMain.Instance.UpdateFighterHp(_opponentHp, 1);
                GroupMain.Instance.UpdateFighterSword(_opponentSword, 1);
                GroupMain.Instance.UpdateFighterCondition(_opponentCondition, 1);
                GroupMain.Instance.UpdateFighterCardQuantity(_opponentCardQuantity, 1);
                GroupMain.Instance.DrawPlayerCards(_playerCardHand, _playerSelectedCard1, _playerSelectedCard2);
                GroupMain.Instance.DrawOpponentCards(_opponentCardHand);
            });
        
        _coroutines.Add(StartCoroutine(CoroutineServerConnection()));
    }
    
    public override void OnConnectedToMaster()
    {
        _isConnected = true;
        
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2
        };

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        IFighter fighter = PhotonNetwork.Instantiate("Fighter", Vector3.zero, Quaternion.identity)
            .GetComponent<IFighter>();

        fighter.Initialize(_playerUserName, _playerAvatarId, _rawCardDeck);
        
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        _coroutines.Add(StartCoroutine(CoroutineMatchMaking()));
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        foreach (Coroutine coroutine in _coroutines)
        {
            StopCoroutine(coroutine);
        }

        _phrase = MainScenePhrase.Idle;

        _fighterPlayer = null;
        _fighterOpponent = null;

        GroupConnection.Instance.HideServerConnection();
        GroupConnection.Instance.HideMatchMaking();
        GroupConnection.Instance.HideOpponentInformation();
        GroupConnection.Instance.HideCountDown();

        if (_hasOpponentRun)
        {
            SoundPlayer.Instance.PlaySound(3);
            ResultModel.Instance.SetResult(GameResult.Win);
            string textMessage = _opponentUserName + "は、しっぽを巻いて逃げたようです";
            GroupDisconnect.Instance.ShowForm("通信が途絶えました", textMessage);
            Observable.Timer(TimeSpan.FromSeconds(5.0f)).Subscribe(__ =>
            {
                GroupFadeEffect.Instance.FadeIn();
            });
        }
        else
        {
            switch (cause)
            {
                case DisconnectCause.MaxCcuReached:
                    GroupDisconnect.Instance.ShowForm("接続に失敗しました","最大接続人数に達しました");
                    GroupConnection.Instance.EnableReconnectButton();
                    break;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_isMatched)
        {
            _hasOpponentRun = true;
            _phrase = MainScenePhrase.Idle;
            PhotonNetwork.Disconnect();
        }
    }

    IEnumerator CoroutineServerConnection()
    {
        _isConnected = false;
        GroupConnection.Instance.DisableReconnectButton();

        PhotonNetwork.ConnectUsingSettings();

        int count = 0;
        GroupConnection.Instance.ShowServerConnection();
        while (!_isConnected)
        {
            GroupConnection.Instance.DrawServerConnection(count);
            yield return new WaitForSeconds(0.5f);
            count++;
        }
        GroupConnection.Instance.HideServerConnection();
    }

    IEnumerator CoroutineMatchMaking()
    {
        _isMatched = false;
        
        int count = 0;
        GroupConnection.Instance.ShowMatchMaking();
        GroupDisconnect.Instance.HideForm();
        while (!_isMatched)
        {
            GroupConnection.Instance.DrawMatchMaking(count);
            yield return new WaitForSeconds(0.5f);
            GameObject[] gameObjectsFighters = GameObject.FindGameObjectsWithTag("Fighter");
            if (gameObjectsFighters.Length == 2)
            {
                SoundPlayer.Instance.PlaySound(4);
                _isMatched = true;
                foreach (GameObject gameObjectFighter in gameObjectsFighters)
                {
                    IFighter fighter = gameObjectFighter.GetComponent<IFighter>();
                    if (fighter == null) continue;
                    if (fighter.IsPlayer())
                    {
                        _fighterPlayer = fighter;
                    }

                    if (fighter.IsOpponent())
                    {
                        _fighterOpponent = fighter;
                    }
                }

                GroupConnection.Instance.DisableReturnButton();
            }
            count++;
        }

        if (_fighterOpponent != null)
        {
            _opponentUserName = _fighterOpponent.UserName;
            _opponentAvatarId = _fighterOpponent.AvatarId;
        }
        
        GroupConnection.Instance.HideMatchMaking();
        GroupConnection.Instance.ShowOpponentInformation(_opponentUserName, _opponentAvatarId);

        yield return new WaitForSeconds(5.0f);

        int timer = 3;
        GroupConnection.Instance.ShowCountDown();
        while (timer > 0)
        {
            GroupConnection.Instance.DrawCountDown(timer);
            yield return new WaitForSeconds(1.0f);
            timer--;
        }

        GroupConnection.Instance.DrawCountDown(0);
        
        
        GroupSceneEffect.Instance.Hide();
    }
    

    IEnumerator CoroutineCountDownCard()
    {
        int count = 100;
        GroupMain.Instance.ShowDiscardNotice(_hasPlayerCard, _playerCondition);
        GroupMain.Instance.ShowDiscardTimer();
        _isSelectable = true;
        while (count > 0)
        {
            GroupMain.Instance.DrawDiscardTimer(count);
            yield return new WaitForSeconds(0.1f);
            count--;
        }

        _isSelectable = false;
        GroupMain.Instance.HideDiscardNotice();
        GroupMain.Instance.DrawDiscardTimer(0);
        yield return new WaitForSeconds(0.3f);
        
        _fighterPlayer?.SetReadyCode(3);
    }
    IEnumerator CoroutineStartHands()
    {
        if (_fighterPlayer != null)
        {
            for (int i = 0; i < 6; i++)
            {
                yield return new WaitForSeconds(0.6f);
                _fighterPlayer.DrawCard(i);
            }
        }
        
        yield return new WaitForSeconds(0.5f);

        _fighterPlayer?.SetReadyCode(1);
    }

    //極めて深いコルーチンになる、要注意。
    IEnumerator CoroutineProcessActionQueries()
    {
        List<ActionQuery> queries = ActionQueryModel.Instance.GenerateActionQueries(new QueryInformation
        {
            card1 = _playerSelectedCard1 >= 0 ? _playerCardHand[_playerSelectedCard1] : -1,
            card2 = _playerSelectedCard2 >= 0 ? _playerCardHand[_playerSelectedCard2] : -1,
            sword = _playerSword,
            condition = _playerCondition
        }, new QueryInformation
        {
            card1 = _opponentSelectedCard1 >= 0 ? _opponentCardHand[_opponentSelectedCard1] : -1,
            card2 = _opponentSelectedCard2 >= 0 ? _opponentCardHand[_opponentSelectedCard2] : -1,
            sword = _opponentSword,
            condition = _opponentCondition
        });
        GroupMain.Instance.InitializeAnnounceBoard();
        yield return new WaitForSeconds(1.0f);
        GroupMain.Instance.ShowAnnounceBoard();
        yield return new WaitForSeconds(1.0f);
        GroupMain.Instance.ClearFieldCards(_playerSelectedCard1, _playerSelectedCard2, _opponentSelectedCard1,
            _opponentSelectedCard2);

        int queryPlayerHp = _playerHp;
        int queryOpponentHp = _opponentHp;
        foreach (ActionQuery query in queries)
        {
            string text = "";
            int sound = 0;
            yield return new WaitForSeconds(1.0f);
            switch (query.type)
            {
                case QueryType.Nothing:
                    text = "何も起こらない！";
                    sound = 12;
                    break;

                case QueryType.SlashAttack:
                    if (query.isPlayer)
                    {
                        text = _playerUserName + "は、" + _opponentUserName + "に向かって剣を振った！";
                    }
                    else
                    {
                        text = _opponentUserName + "は、" + _playerUserName + "を斬りかかった！";
                    }

                    sound = 13;
                    break;
                
                case QueryType.Guard:
                    if (query.isPlayer)
                    {
                        text = _playerUserName + "は、盾を前に出した！";
                    }
                    else
                    {
                        text = _opponentUserName + "は、盾を向ける！";
                    }
                    
                    break;
                
                case QueryType.MagicAttack:
                    if (query.isPlayer)
                    {
                        text = _playerUserName + "は、" + _opponentUserName + "に向かって魔法攻撃！";
                    }
                    else
                    {
                        text = _opponentUserName + "は、" + _playerUserName + "に向かって魔法攻撃！";
                    }

                    sound = 14;
                    break;
                
                case QueryType.MagicReflector:
                    if (query.isPlayer)
                    {
                        text = _playerUserName + "のマジックリフレクター！";
                    }
                    else
                    {
                        text = _opponentUserName + "のマジックリフレクター！";
                    }

                    sound = 16;
                    break;
                
                case QueryType.Damage:
                    if (query.isPlayer)
                    {
                        if (query.value > 0)
                        {
                            queryPlayerHp -= query.value;
                            if (queryPlayerHp < 0)
                            {
                                queryPlayerHp = 0;
                            }

                            _fighterPlayer?.SetHp(queryPlayerHp);
                            GroupMain.Instance.UpdateFighterHp(queryPlayerHp, 0);
                            text = _playerUserName + "は、" + query.value + "のダメージを受けた！";
                        }
                        else
                        {
                            text = _playerUserName + "は、ちっとも痛くないようだ。";
                        }
                    }
                    else
                    {
                        if (query.value > 0)
                        {
                            queryOpponentHp -= query.value;
                            if (queryOpponentHp < 0)
                            {
                                queryOpponentHp = 0;
                            }
                            
                            GroupMain.Instance.UpdateFighterHp(queryOpponentHp, 1);
                            text = _opponentUserName + "に、" + query.value + "のダメージを与えた！";
                        }
                        else
                        {
                            text = _opponentUserName + "は、全く痛がっていない！";
                        }
                    }

                    if (query.value >= 6)
                    {
                        sound = 11;
                    }
                    else if (query.value >= 4)
                    {
                        sound = 10;
                    }
                    else if (query.value >= 2)
                    {
                        sound = 9;
                    }
                    else if (query.value >= 1)
                    {
                        sound = 8;
                    }
                    else
                    {
                        sound = 12;
                    }
                    
                    break;
                
                case QueryType.UsePotion:
                    if (query.isPlayer)
                    {
                        text = _playerUserName + "は、ポーションを飲んだ！";
                    }
                    else
                    {
                        text = _opponentUserName + "は、ポーションを飲んだ！";
                    }

                    sound = 17;
                    break;
                
                case QueryType.RecoveryBlock:
                    if (query.isPlayer)
                    {
                        text = _playerUserName + "は、回復封じの魔法を使った！";
                    }
                    else
                    {
                        text = _opponentUserName + "は、回復封じの魔法を使った！";
                    }

                    sound = 14;
                    break;
                
                case QueryType.Recovery:
                    if (query.isPlayer)
                    {
                        if (query.value > 0)
                        {
                            queryPlayerHp += query.value;
                            if (queryPlayerHp > 10)
                            {
                                queryPlayerHp = 10;
                            }

                            _fighterPlayer?.SetHp(queryPlayerHp);
                            GroupMain.Instance.UpdateFighterHp(queryPlayerHp, 0);
                            text = _playerUserName + "のHPが、" + query.value + "回復した！";
                        }
                        else
                        {
                            text = _playerUserName + "の傷は、全然治らない！";
                        }
                    }
                    else
                    {
                        if (query.value > 0)
                        {
                            queryOpponentHp += query.value;
                            if (queryOpponentHp > 10)
                            {
                                queryOpponentHp = 10;
                            }
                            
                            GroupMain.Instance.UpdateFighterHp(queryOpponentHp, 1);
                            text = _opponentUserName + "のHPが、" + query.value + "回復した！";
                        }
                        else
                        {
                            text = _opponentUserName + "の傷は、全然治っていない";
                        }
                    }

                    if (query.value >= 1)
                    {
                        sound = 15;
                    }
                    else
                    {
                        sound = 12;
                    }
                    break;
                
                case QueryType.Enchant:
                    if (query.isPlayer)
                    {
                        switch (query.element)
                        {
                            case QueryElement.Normal:
                                _playerSword = FighterSword.Normal;
                                text = _playerUserName + "の剣は元に戻った";
                                break;
                            case QueryElement.Fire:
                                _playerSword = FighterSword.Fire;
                                text = _playerUserName + "の剣に、炎属性エンチャント！";
                                break;
                            case QueryElement.Thunder:
                                _playerSword = FighterSword.Thunder;
                                text = _playerUserName + "の剣に、雷属性エンチャント！";
                                break;
                            case QueryElement.Ice:
                                _playerSword = FighterSword.Ice;
                                text = _playerUserName + "の剣に、氷属性エンチャント！";
                                break;
                        }
                        _fighterPlayer?.SetSword(_playerSword);
                        GroupMain.Instance.UpdateFighterSword(_playerSword, 0);
                    }
                    else
                    {
                        switch (query.element)
                        {
                            case QueryElement.Normal:
                                _opponentSword = FighterSword.Normal;
                                text = _opponentUserName + "の剣は元に戻った";
                                break;
                            case QueryElement.Fire:
                                _opponentSword = FighterSword.Fire;
                                text = _opponentUserName + "の剣に、炎属性エンチャント！";
                                break;
                            case QueryElement.Thunder:
                                _opponentSword = FighterSword.Thunder;
                                text = _opponentUserName + "の剣に、雷属性エンチャント！";
                                break;
                            case QueryElement.Ice:
                                _opponentSword = FighterSword.Ice;
                                text = _opponentUserName + "の剣に、氷属性エンチャント！";
                                break;
                        }
                        GroupMain.Instance.UpdateFighterSword(_opponentSword, 1);
                    }

                    switch (query.element)
                    {
                        case QueryElement.Fire:
                            sound = 18;
                            break;
                        case QueryElement.Thunder:
                            sound = 19;
                            break;
                        case QueryElement.Ice:
                            sound = 20;
                            break;
                    }
                    
                    break;
                                
                case QueryType.Debuff:
                    if (query.isPlayer)
                    {
                        switch (query.element)
                        {
                            case QueryElement.Normal:
                                _playerCondition = FighterCondition.Fine;
                                text = _playerUserName + "は正常になった！";
                                break;
                            case QueryElement.Fire:
                                _playerCondition = FighterCondition.Burned;
                                text = _playerUserName + "は火傷してしまった！";
                                break;
                            case QueryElement.Thunder:
                                _playerCondition = FighterCondition.Paralysis;
                                text = _playerUserName + "は麻痺してしまった！";
                                break;
                            case QueryElement.Ice:
                                _playerCondition = FighterCondition.Frozen;
                                text = _playerUserName + "は凍結してしまった！";
                                break;
                        }
                        _fighterPlayer?.SetCondition(_playerCondition);
                        GroupMain.Instance.UpdateFighterCondition(_playerCondition, 0);
                    }
                    else
                    {
                        switch (query.element)
                        {
                            case QueryElement.Normal:
                                _opponentCondition = FighterCondition.Fine;
                                text = _opponentUserName + "は正常に戻った";
                                break;
                            case QueryElement.Fire:
                                _opponentCondition = FighterCondition.Burned;
                                text = _opponentUserName + "は火傷した！";
                                break;
                            case QueryElement.Thunder:
                                _opponentCondition = FighterCondition.Paralysis;
                                text = _opponentUserName + "は麻痺した！";
                                break;
                            case QueryElement.Ice:
                                _opponentCondition = FighterCondition.Frozen;
                                text = _opponentUserName + "は凍結した！";
                                break;
                        }
                        GroupMain.Instance.UpdateFighterCondition(_opponentCondition, 1);
                    }
                    break;
            }
            GroupMain.Instance.DrawAnnounceBoard(text);
            if (sound > 0)
            {
                SoundPlayer.Instance.PlaySound(sound);
            }
        }

        yield return new WaitForSeconds(2.0f);
        GroupMain.Instance.HideAnnounceBoard();
        yield return new WaitForSeconds(1.0f);
        _fighterPlayer?.SetReadyCode(5);
    } 
}
