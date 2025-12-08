using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MenuUI;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI manualText;
    public TextMeshProUGUI copyrightText;
    public TextMeshProUGUI versionText;
    public GameObject optionsScreen;
    public Menu[] menu;

    bool _isOtherScreenOpening;
    int _currentMenuIndex = 0;
    bool _menuInitialized = false;

    void Start()
    {
        if (manualText == null)
        {
            GameObject manualTextObj = GameObject.Find("ManualText");
            if (manualTextObj != null)
                manualText = manualTextObj.GetComponent<TextMeshProUGUI>();
        }
        if (copyrightText == null)
        {
            GameObject copyrightTextObj = GameObject.Find("CopyrightText");
            if (copyrightTextObj != null)
                copyrightText = copyrightTextObj.GetComponent<TextMeshProUGUI>();
        }
        if (versionText == null)
        {
            GameObject versionTextObj = GameObject.Find("VersionText");
            if (versionTextObj != null)
                versionText = versionTextObj.GetComponent<TextMeshProUGUI>();
        }

        SortMenuByPosition();
        _currentMenuIndex = 0;
        _menuInitialized = false;
        TitleTextRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);

        if (optionsScreen != null)
        {
            var optionsScreenComponent = optionsScreen.GetComponent<OptionsScreen>();
            if (optionsScreenComponent != null)
                optionsScreenComponent.PrevScreenReturn += OnPrevScreenReturn;
        }
    }

    void SortMenuByPosition()
    {
        System.Array.Sort(menu, (a, b) =>
        {
            if (a.text == null || a.text.Length == 0 || a.text[0] == null) return 1;
            if (b.text == null || b.text.Length == 0 || b.text[0] == null) return -1;
            return b.text[0].transform.position.y.CompareTo(a.text[0].transform.position.y);
        });
    }

    void Update()
    {
        if (_isOtherScreenOpening) return;

        if (!_menuInitialized)
        {
            for (int i = 0; i < menu.Length; i++)
            {
                if (menu[i].text != null && menu[i].text.Length > 0 && menu[i].text[0] != null)
                {
                    if (menu[i].text[0].name == "ExitText")
                    {
                        _currentMenuIndex = i;
                        break;
                    }
                }
            }
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            _menuInitialized = true;
        }

        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        bool selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);

        if (upInput)
        {
            _currentMenuIndex--;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        }
        else if (downInput)
        {
            _currentMenuIndex++;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        }
        else if (selectInput)
        {
            if (_currentMenuIndex >= 0 && _currentMenuIndex < menu.Length)
            {
                string menuName = "";
                if (menu[_currentMenuIndex].text != null && menu[_currentMenuIndex].text.Length > 0 && menu[_currentMenuIndex].text[0] != null)
                {
                    menuName = menu[_currentMenuIndex].text[0].name;
                }

                if (menuName == "StartText")
                {
                    GameExit();
                }
                else if (menuName == "ExitText")
                {
                    GameStart();
                }
            }
        }
    }

    public void OpenOtherScreen(GameObject openScreen)
    {
        _isOtherScreenOpening = true;
        if (title != null)
            title.alpha = 0;
        if (versionText != null)
            versionText.alpha = 0;
        if (copyrightText != null)
            copyrightText.alpha = 0;
        MenuUIController.AllMenuTextHide(menu);
        openScreen.SetActive(true);
    }

    void OnPrevScreenReturn()
    {
        if (title != null)
            title.alpha = 1;
        if (versionText != null)
            versionText.alpha = 1;
        if (copyrightText != null)
            copyrightText.alpha = 1;
        _isOtherScreenOpening = false;
        TitleTextRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
    }

    public void GameStart()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SetGameState(GameManager.GameState.Play);

            GameManager.instance.playerStartPos = Vector2.zero;
            GameManager.instance.playerResurrectionPos = Vector2.zero;
            GameManager.instance.playerStartlocalScaleX = 0;
            GameManager.instance.firstStart = true;

            if (SceneTransition.instance != null)
                SceneTransition.instance.LoadScene("OldMachineRoom_A");
        }
        else
        {
            if (SceneTransition.instance != null)
                SceneTransition.instance.LoadScene("OldMachineRoom_A");
        }
    }

    public void GameExit()
    {
        OptionsData.OptionsSave();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    void TitleTextRefresh()
    {
        for(int i = 0; i < menu.Length; i++)
        {
            if (menu[i].text != null && menu[i].text.Length > 0 && menu[i].text[0] != null)
            {
                switch(menu[i].text[0].name)
                {
                    case "StartText":
                        menu[i].text[0].text = LanguageManager.GetText("Start");
                        break;
                    case "OptionsText":
                        menu[i].text[0].text = LanguageManager.GetText("Options");
                        break;
                    case "ExitText":
                        menu[i].text[0].text = LanguageManager.GetText("Exit");
                        break;
                }
            }
        }
    }
}
