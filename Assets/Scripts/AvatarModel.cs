using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AvatarModel : SingletonMonoBehaviour<AvatarModel>
{
    [SerializeField] private string avatarKey;
    private readonly ReactiveProperty<int> _avatarId = new ReactiveProperty<int>();
    public IObservable<int> AvatarId => _avatarId;

    private void Start()
    {
        if (ES3.KeyExists(avatarKey))
        {
            _avatarId.Value = ES3.Load<int>(avatarKey);
        }
        else
        {
            _avatarId.Value = 0;
            ES3.Save(avatarKey, 0);
        }
    }

    public void SetAvatarId(int avatarId)
    {
        _avatarId.Value = avatarId;
        ES3.Save(avatarKey, avatarId);
    }
}
