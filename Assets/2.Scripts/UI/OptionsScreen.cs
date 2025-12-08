using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;

public class OptionsScreen : MonoBehaviour
{

    public delegate void PrevScreenReturnEventHandler();
    public PrevScreenReturnEventHandler PrevScreenReturn;

    public GameObject optionsMenuScreen;
    public TextMeshProUGUI optionsText;
    public TextMeshProUGUI manualText;
    public List<Menu> optionMenu;
    int _currentMenuIndex;

    void Awake()
    {
        if (manualText == null)
        {
            manualText = GameObject.Find("ManualText").GetComponent<TextMeshProUGUI>();
        }

        if (GameManager.instance.currentGameState == GameManager.GameState.Title)
        {
            List<int> menuToRemove = new List<int>();
            for (int i = optionMenu.Count - 1; i >= 0; i--)
            {
                if (optionMenu[i].text[0].name.Equals("ReturnToTitleScreenText") || optionMenu[i].text[0].name.Equals("QuitToDesktopText"))
                {
                    menuToRemove.Add(i);
                    if (menuToRemove.Count >= 2) break;
                }
            }
            for (int i = 0; i < menuToRemove.Count; i++)
            {
                MenuUIController.SelectMenuTextHide(optionMenu[menuToRemove[i]]);
                optionMenu.RemoveAt(menuToRemove[i]);
            }
        }
    }

    private void OnEnable()
    {

        OptionTextRefresh();
        MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);
    }

    void Update()
    {

        if (!optionsMenuScreen.activeSelf) return;

        OptionTextRefresh();
        MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);

        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);
        bool selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);

        if (upInput)
        {

            _currentMenuIndex--;
            MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);
        }
        else if (downInput)
        {

            _currentMenuIndex++;
            MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);
        }
        if (selectInput)
        {

            optionMenu[_currentMenuIndex].menuSelectEvent.Invoke();
        }

        if (GameManager.instance.currentGameState == GameManager.GameState.Title)
        {
            if (backInput)
            {
                _currentMenuIndex = 0;
                gameObject.SetActive(false);
                PrevScreenReturn?.Invoke();
                OptionsData.OptionsSave();
                MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);
            }
        }
    }

    public void ReturnToTitleScreen()
    {
        GameManager.instance.SetGameState(GameManager.GameState.Title);

        GameManager.instance.playerStartPos = Vector2.zero;
        GameManager.instance.playerStartlocalScaleX = 0;
        GameManager.instance.playerResurrectionPos = Vector2.zero;
        GameManager.instance.playerCurrentHealth = 0;
        GameManager.instance.resurrectionScene = string.Empty;
        GameManager.instance.firstStart = true;

        DeadEnemyManager.ClearDeadBosses();
        DeadEnemyManager.ClearDeadEnemies();
        TutorialManager.SeenTutorialClear();
        MapManager.ClearDiscoveredMaps();

        SceneTransition.instance.LoadScene("Title");
    }

    void ReturnToGamePlay()
    {
        _currentMenuIndex = 0;
        GameManager.instance.SetGameState(GameManager.GameState.Play);
        MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);
        gameObject.SetActive(false);
        OptionsData.OptionsSave();
        PrevScreenReturn?.Invoke();
    }

    public void GoToNextScreen(GameObject nextScreen)
    {
        nextScreen.SetActive(true);
        optionsMenuScreen.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    void OptionTextRefresh()
    {
        optionsText.text = LanguageManager.GetText("Options");
        for (int i = 0; i < optionMenu.Count; i++)
        {
            switch (optionMenu[i].text[0].name)
            {
                case "VideoText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("Video");
                    break;
                case "SoundText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("Sound");
                    break;
                case "ControlsText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("Controls");
                    break;
                case "AccessibilityText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("Accessibility");
                    break;
                case "LanguageText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("Language");
                    break;
                case "ReturnToTitleScreenText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("ReturnToTitleScreen");
                    break;
                case "QuitToDesktopText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("QuitToDesktop");
                    break;
            }
        }
    }
}
