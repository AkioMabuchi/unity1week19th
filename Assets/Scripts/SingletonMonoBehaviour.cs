using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
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
