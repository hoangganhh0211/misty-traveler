using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionsSaveData
{
    public bool fullScreenMode = true;
    public Resolution? resolution = null;
    public bool vSync = true;
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float effectsVolume = 1.0f;
    public float gamepadVibration = 1.0f;
    public bool screenShake = true;
    public bool screenFlashes = true;
    public List<int> keyMapping = new List<int>();
    public List<int> buttonMapping = new List<int>();
}
