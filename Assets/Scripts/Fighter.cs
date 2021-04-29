using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UniRx;

public interface IFighter
{
    public void Initialize(string userName, int avatarId);
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
}
public class Fighter : MonoBehaviourPunCallbacks, IPunObservable, IFighter
{
    private PhotonView _photonView;

    private string _userName;
    private int _avatarId;

    private bool _isBurned;
    private bool _isParalysis;
    private bool _isFrozen;

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
        }
        else
        {
            _userName = (string) stream.ReceiveNext();
            _avatarId = (int) stream.ReceiveNext();
        }
    }

    public void Initialize(string userName, int avatarId)
    {
        _userName = userName;
        _avatarId = avatarId;
    }

    public bool IsPlayer()
    {
        return _photonView.IsMine;
    }

    public bool IsOpponent()
    {
        return !_photonView.IsMine;
    }
}
