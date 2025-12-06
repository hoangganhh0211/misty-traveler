using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class SoundSettingsManager
{
    public static float masterVolume = 1.0f;
    public static float musicVolume = 1.0f;
    public static float effectsVolume = 1.0f;

    public static void Init()
    {

        masterVolume = OptionsData.optionsSaveData.masterVolume;
        musicVolume = OptionsData.optionsSaveData.musicVolume;
        effectsVolume = OptionsData.optionsSaveData.effectsVolume;

        AudioListener.volume = masterVolume;
    }

    public static void SetMasterVolume(bool increase)
    {

        if (increase)
        {
            if (masterVolume < 1f)
            {
                masterVolume += 0.1f;
            }
        }
        else
        {
            if (masterVolume > 0f)
            {
                masterVolume -= 0.1f;
            }
        }

        AudioListener.volume = masterVolume;
        OptionsData.optionsSaveData.masterVolume = masterVolume;
    }

    public static string GetMasterVolumeToTextUI()
    {
        int volume = Mathf.RoundToInt(AudioListener.volume * 10);
        StringBuilder volumeToText = new StringBuilder();
        for (int i = 0; i < 10; i++)
        {
            if (i < volume)
            {
                volumeToText.Append("■");
            }
            else
            {
                volumeToText.Append("□");
            }
        }
        return volumeToText.ToString();
    }

    public static void SetMusicVolume(bool increase)
    {
        if(increase)
        {
            if(musicVolume < 1f)
            {
                musicVolume += 0.1f;
            }
        }
        else
        {
            if (musicVolume > 0f)
            {
                musicVolume -= 0.1f;
            }
        }

        OptionsData.optionsSaveData.musicVolume = musicVolume;
    }

    public static string GetMusicVolumeToTextUI()
    {
        int volume = Mathf.RoundToInt(musicVolume * 10);
        StringBuilder volumeToText = new StringBuilder();
        for (int i = 0; i < 10; i ++)
        {
            if(i < volume)
            {
                volumeToText.Append("■");
            }
            else
            {
                volumeToText.Append("□");
            }
        }
        return volumeToText.ToString();
    }

    public static void SetEffectsVolume(bool increase)
    {
        if (increase)
        {
            if (effectsVolume < 1)
            {
                effectsVolume += 0.1f;
            }
        }
        else
        {
            if (effectsVolume > 0)
            {
                effectsVolume -= 0.1f;
            }
        }

        OptionsData.optionsSaveData.effectsVolume = effectsVolume;
    }

    public static string GetEffectsVolumeToTextUI()
    {
        int volume = Mathf.RoundToInt(effectsVolume * 10);
        StringBuilder volumeToText = new StringBuilder();
        for (int i = 0; i < 10; i++)
        {
            if (i < volume)
            {
                volumeToText.Append("■");
            }
            else
            {
                volumeToText.Append("□");
            }
        }
        return volumeToText.ToString();
    }
}