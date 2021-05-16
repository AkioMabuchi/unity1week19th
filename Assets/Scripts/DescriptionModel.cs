using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DescriptionModel : SingletonMonoBehaviour<DescriptionModel>
{
    [SerializeField] private string descriptionKey;
    private readonly ReactiveProperty<bool> _isDescriptionActive = new ReactiveProperty<bool>();
    public IObservable<bool> IsDescriptionActive => _isDescriptionActive;

    private void Start()
    {
        if (ES3.KeyExists(descriptionKey))
        {
            _isDescriptionActive.Value = ES3.Load<bool>(descriptionKey);
        }
        else
        {
            _isDescriptionActive.Value = true;
            ES3.Save(descriptionKey, true);
        }
    }

    public void SetDescription(bool s)
    {
        _isDescriptionActive.Value = s;
        ES3.Save(descriptionKey, s);
    }
}
