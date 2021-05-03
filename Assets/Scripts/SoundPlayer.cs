using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : SingletonMonoBehaviour<SoundPlayer>
{
    [SerializeField] private GameObject prefabSound;

    [SerializeField] private AudioClip[] audioClips;
    
    public void PlaySound(int index)
    {
        Instantiate(prefabSound, Vector3.zero, Quaternion.identity, transform).GetComponent<Sound>()
            .Initialize(audioClips[index]);
    }
}
