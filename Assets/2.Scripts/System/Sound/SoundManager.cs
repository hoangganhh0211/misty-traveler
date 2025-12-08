using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    [SerializeField] AudioSource _music;
    [SerializeField] AudioSource _effect;

    float currentMusicVolume;
    AudioClip _musicClip;
    Coroutine _musicChange = null;

    void Awake()
    {

        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if(_music == null)
        {
            _music = transform.Find("Music").GetComponent<AudioSource>();
        }
        if(_effect == null)
        {
            _effect = transform.Find("Effect").GetComponent<AudioSource>();
        }

        _music.loop = true;
    }

    public void MusicPlay(AudioClip audioClip, float volume = 1.0f)
    {
        if(audioClip == null || _musicClip == audioClip) return;
        _musicClip = audioClip;

        if(_musicChange != null)
        {
            StopCoroutine(_musicChange);
            _musicChange =null;
        }

        _musicChange = StartCoroutine(MusicChange(volume));
    }

    public void MusicStop()
    {

        if (_musicChange != null)
        {
            StopCoroutine(_musicChange);
            _musicChange = null;
        }
        _musicClip = null;
        _musicChange = StartCoroutine(MusicChange(0));
    }

    public AudioClip GetCurrentMusic() => _music.clip;

    IEnumerator MusicChange(float setVolume)
    {

        currentMusicVolume = SoundSettingsManager.musicVolume;

        float volume = _music.volume;
        float volumeReduction = volume * 0.015f;

        while(volume > 0f)
        {
            volume -= volumeReduction;
            _music.volume = Mathf.Clamp(volume, 0f, 1.0f);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        _music.Stop();

        _music.clip = _musicClip;
        _music.Play();

        float volumeIncrease = setVolume * 0.015f;
        while (volume < setVolume * currentMusicVolume)
        {
            volume += volumeIncrease;
            _music.volume = Mathf.Clamp(volume, 0f, setVolume * currentMusicVolume);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        if(currentMusicVolume != SoundSettingsManager.musicVolume)
        {
            _music.volume = setVolume * SoundSettingsManager.musicVolume;
        }

        _musicChange = null;
    }

    public void SoundEffectPlay(AudioClip audioClip)
    {
        if (audioClip == null) return;
        
        _effect.volume = SoundSettingsManager.effectsVolume;
        _effect.PlayOneShot(audioClip);
    }

    public void MusicVolumeRefresh()
    {

        if (_musicChange != null)
        {
            StopCoroutine(_musicChange);
            _musicChange = null;
        }
        _music.volume = currentMusicVolume * SoundSettingsManager.musicVolume;
    }
}
