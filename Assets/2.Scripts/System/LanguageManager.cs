using System.Collections.Generic;

public static class LanguageManager
{
    private static Dictionary<string, string> translations = new Dictionary<string, string>()
    {

        {"Options", "Options"},
        {"Video", "Video"},
        {"Sound", "Sound"},
        {"Controls", "Controls"},
        {"Accessibility", "Accessibility"},
        {"Language", "Language"},
        {"ReturnToTitleScreen", "Return to Title"},
        {"QuitToDesktop", "Quit to Desktop"},
        {"Start", "Start"},
        {"Exit", "Exit"},
        {"Map", "Map"},
        {"Empty", ""},
        {"Actions", "Actions"},
        {"Keyboard", "Keyboard"},

        {"FullScreen", "Full Screen"},
        {"Resolution", "Resolution"},
        {"VSync", "VSync"},
        {"Enabled", "Enabled"},
        {"Disabled", "Disabled"},

        {"MasterVolume", "Master Volume"},
        {"MusicVolume", "Music Volume"},
        {"EffectsVolume", "Effects Volume"},

        {"MoveLeft", "Move Left"},
        {"MoveRight", "Move Right"},
        {"MoveUp", "Move Up"},
        {"MoveDown", "Move Down"},
        {"Jump", "Jump"},
        {"Attack", "Attack"},
        {"ResetToDefault", "Reset to Default"},

        {"ControllerVibration", "Controller Vibration"},
        {"ScreenShake", "Screen Shake"},
        {"ScreenFlashes", "Screen Flashes"},

        {"Back", "Back"},
        {"Confirm", "Confirm"},
        {"Delete", "Delete"},
        {"SelectProfile", "Select Profile"}
    };

    public static void Init()
    {

    }

    public static string GetText(string key)
    {
        if (translations.ContainsKey(key))
            return translations[key];
        return key;
    }
}
