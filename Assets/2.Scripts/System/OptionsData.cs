

public static class OptionsData
{
    public static OptionsSaveData optionsSaveData;
    static bool hasInit;

    public static void Init()
    {

        if (!hasInit)
        {
            hasInit = true;

            optionsSaveData = SaveSystem.OptionLoad();
            if (optionsSaveData == null)
            {
                optionsSaveData = new OptionsSaveData();
            }

            VideoSettingsManager.VideoOptionsInit();
            GameInputManager.Init();
            SoundSettingsManager.Init();
            AccessibilitySettingsManager.Init();
            LanguageManager.Init();
        }
    }

    public static void OptionsSave()
    {
        SaveSystem.OptionSave(optionsSaveData);
    }
}