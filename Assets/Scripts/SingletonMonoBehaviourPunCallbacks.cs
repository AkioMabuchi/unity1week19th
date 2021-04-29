using System;
using Photon.Pun;
using UnityEngine;

public class SingletonMonoBehaviourPunCallbacks<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Type t = typeof(T);
                _instance = (T) FindObjectOfType(t);
                if (_instance == null)
                {
                    Debug.LogError(t + "をアタッチしているGameObjectはありません");
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        CheckInstance();
    }

    void CheckInstance()
    {
        if (_instance == null)
        {
            _instance = this as T;
            return;
        }

        if (_instance == this)
        {
            return;
        }

        Destroy(this);
    }
}
