using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class PresenterMonoBehaviourPunCallbacks : MonoBehaviourPunCallbacks
{
    protected readonly List<IDisposable> Disposables = new List<IDisposable>();
    
    private void OnDestroy()
    {
        foreach (IDisposable disposable in Disposables)
        {
            disposable?.Dispose();
        }
    }
}
