using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UniRx;

public enum MainSceneMode
{
    Idle,
    Main,
    
}
// 注意、このクラスは非常に行数が長くなる可能性がある。
public class MainSceneManager : PresenterMonoBehaviourPunCallbacks
{
    private bool _isConnected;
    private bool _isMatched;

    private Coroutine _coroutineServerConnection;
    private Coroutine _coroutineMatchMaking;
    private Coroutine _coroutineCountDown;

    private IReferee _referee;
    private IFighter _fighterPlayer;
    private IFighter _fighterOpponent;

    private string _playerUserName;
    private int _playerAvatarId = 0;
    private string _opponentUserName;
    private int _opponentAvatarId;
    private void Start()
    {
        Disposables.Add(GroupConnection.Instance.OnClickReturn.Subscribe(_ =>
        {
            PhotonNetwork.Disconnect();
            SceneController.Instance.ChangeScene("MainScene", "PrepareScene");
        }));
        
        Disposables.Add(GroupConnection.Instance.OnClickReconnection.Subscribe(_ =>
        {
            _coroutineServerConnection = StartCoroutine(CoroutineServerConnection());
        }));
        
        Disposables.Add(UserNameModel.Instance.UserName.Subscribe(userName =>
        {
            _playerUserName = userName;
            Debug.Log(_playerUserName);
        }));
        
        Disposables.Add(GroupSceneEffect.Instance.OnCompleteHide.Subscribe(_ =>
        {
            GroupConnection.Instance.HideView();
            Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe(__ =>
            {
                GroupSceneEffect.Instance.Show();
            });
        }));
        
        Disposables.Add(GroupSceneEffect.Instance.OnCompleteShow.Subscribe(_ =>
        {
            Debug.Log("対戦開始！");
        }));
        
        PhotonNetwork.GameVersion = "Version 1.0.0";

        _coroutineServerConnection = StartCoroutine(CoroutineServerConnection());
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

        fighter.Initialize(_playerUserName, _playerAvatarId);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.Instantiate("Referee", Vector3.zero, Quaternion.identity);
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        _coroutineMatchMaking = StartCoroutine(CoroutineMatchMaking());
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);

        if (_coroutineServerConnection != null)
        {
            StopCoroutine(_coroutineServerConnection);
        }

        if (_coroutineMatchMaking != null)
        {
            StopCoroutine(_coroutineMatchMaking);
        }

        if (_coroutineCountDown != null)
        {
            StopCoroutine(_coroutineCountDown);
        }
        
        GroupConnection.Instance.HideServerConnection();
        GroupConnection.Instance.HideMatchMaking();
        GroupConnection.Instance.HideOpponentInformation();
        GroupConnection.Instance.HideCountDown();
        GroupSceneEffect.Instance.StopHide();
        GroupSceneEffect.Instance.StopShow();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_isMatched)
        {
            Debug.Log("相手はしっぽを巻いて逃げたようです。");
            PhotonNetwork.Disconnect();
        }
    }

    IEnumerator CoroutineServerConnection()
    {
        _isConnected = false;

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
        while (!_isMatched)
        {
            GroupConnection.Instance.DrawMatchMaking(count);
            yield return new WaitForSeconds(0.5f);
            GameObject[] gameObjectsFighters = GameObject.FindGameObjectsWithTag("Fighter");
            if (gameObjectsFighters.Length == 2)
            {
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
                _referee = GameObject.FindWithTag("Referee").GetComponent<IReferee>();
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

        _coroutineCountDown = StartCoroutine(CoroutineCountDown());
    }

    IEnumerator CoroutineCountDown()
    {
        if (_referee != null)
        {
            _referee.StartCountDown();
            GroupConnection.Instance.ShowCountDown();
            while (_referee.CountDown > 0)
            {
                GroupConnection.Instance.DrawCountDown(_referee.CountDown);
                yield return null;
            }

            GroupConnection.Instance.DrawCountDown(0);
            _referee.StopCountDown();
        }
        
        GroupSceneEffect.Instance.Hide();
    }
}
