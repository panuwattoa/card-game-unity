using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Utils;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
#pragma warning disable 649
    [SerializeField] private new AudioSource audio;
#pragma warning restore 649

    private static GameObject effectManagerObject;


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySoundByName(string soundName)
    {
        AudioClip clip = Resources.Load<AudioClip>("sound/"+soundName);
        audio.clip = clip;
        audio.Play();
    }
    
    
}
