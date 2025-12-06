using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMusic : MonoBehaviour
{
    public AudioClip stageMusic;
    public float volume = 1.0f;

    void Start()
    {

        if(stageMusic == null)
        {
            SoundManager.instance.MusicStop();
        }
        SoundManager.instance.MusicPlay(stageMusic, volume);
    }
}
