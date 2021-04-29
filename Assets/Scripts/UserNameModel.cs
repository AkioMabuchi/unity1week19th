using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UserNameModel : SingletonMonoBehaviour<UserNameModel>
{
    [SerializeField] private string userNameKey;
    private readonly ReactiveProperty<string> _userName = new ReactiveProperty<string>();
    public IObservable<string> UserName => _userName;
    private void Start()
    {
        if (ES3.KeyExists(userNameKey))
        {
            _userName.Value = ES3.Load<string>(userNameKey);
        }
        else
        {
            string userName = "勇者" + UnityEngine.Random.Range(0, 10000).ToString("D4");
            _userName.Value = userName;
            ES3.Save(userNameKey, userName);
        }
    }

    public void SetUserName(string userName)
    {
        _userName.Value = userName;
        ES3.Save(userNameKey, userName);
    }
}
