using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PresenterMonoBehaviour : MonoBehaviour
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
