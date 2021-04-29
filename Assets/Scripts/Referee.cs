using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public interface IReferee
{
    public int CountDown
    {
        get;
    }
    public int GameCount
    {
        get;
    }
    public void StartCountDown();
    public void StopCountDown();
}
public class Referee : SingletonMonoBehaviourPunCallbacks<Referee>, IPunObservable, IReferee
{
    private PhotonView _photonView;

    private int _countDown = 3;
    private int _gameCount = 100;
    
    public int CountDown => _countDown;
    public int GameCount => _gameCount;
    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_countDown);
            stream.SendNext(_gameCount);
        }
        else
        {
            _countDown = (int) stream.ReceiveNext();
            _gameCount = (int) stream.ReceiveNext();
        }
    }

    public void StartCountDown()
    {
        if (_photonView.IsMine)
        {
            StartCoroutine(CoroutineCountDown());
        }
    }

    public void StopCountDown()
    {
        if (_photonView.IsMine)
        {
            
        }
    }

    IEnumerator CoroutineCountDown()
    {
        _countDown = 3;
        while (_countDown > 0)
        {
            yield return new WaitForSeconds(1.0f);
            _countDown--;
        }
    }
}
