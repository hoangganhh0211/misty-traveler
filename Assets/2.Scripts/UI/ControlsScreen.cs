using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ControlsScreen : MonoBehaviour
{
    public GameObject optionsMenuScreen;
    public TextMeshProUGUI controlsText;
    public TextMeshProUGUI actionsText;
    public TextMeshProUGUI keyboardText;
    public TextMeshProUGUI manualText;
    public Menu[] menu;

    int _currentMenuIndex;
    bool _isMapping;

    void Awake()
    {
        if (manualText == null)
        {
            manualText = GameObject.Find("ManualText").GetComponent<TextMeshProUGUI>();
        }

        KeyRefresh();
    }

    void OnEnable()
    {
        ControlsOptionsRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        SetNonSelectedDeviceTextColor();
    }

    void Update()
    {

        if (_isMapping) return;

        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        bool selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);

        if (upInput)
        {

            _currentMenuIndex--;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            SetNonSelectedDeviceTextColor();
        }
        else if (downInput)
        {

            _currentMenuIndex++;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            SetNonSelectedDeviceTextColor();
        }

        if (selectInput)
        {

            menu[_currentMenuIndex].menuSelectEvent.Invoke();
            SetNonSelectedDeviceTextColor();
        }

        if (backInput)
        {

            _currentMenuIndex = 0;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            ReturnToOptionsMenuScreen();
        }
    }

    public void SetButtonMapping(string action)
    {
        var stringToAction = (GameInputManager.PlayerActions)Enum.Parse(typeof(GameInputManager.PlayerActions), action);
        StartCoroutine(ChooseButtonToMap(stringToAction));
    }

    IEnumerator ChooseButtonToMap(GameInputManager.PlayerActions action)
    {
        _isMapping = true;

        TextMeshProUGUI buttonText = menu[_currentMenuIndex].text[1];
        buttonText.color = new Color32(141, 105, 122, 255);
        manualText.text = LanguageManager.GetText("Empty");

        yield return null;

        while(true)
        {
            if (GameInputManager.usingController)
            {

                GamepadButton inputButton = GameInputManager.GetCurrentInputButton();

                if (inputButton != GamepadButton.Start)
                {
                    GameInputManager.GamepadMapping(action, inputButton);
                    break;
                }
            }
            else
            {

                Key inputKey = GameInputManager.GetCurrentInputKey();

                if(inputKey != Key.None)
                {
                    GameInputManager.KeyboardMapping(action, inputKey);
                    KeyRefresh();
                    break;
                }
            }
            yield return null;
        }

        MenuUIController.SelectMenuTextColorChange(menu[_currentMenuIndex]);
        _isMapping = false;

        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        SetNonSelectedDeviceTextColor();
    }

    public void ReturnToDefault()
    {
        GameInputManager.KeyboardInputSetDefaults();
        KeyRefresh();
    }

    void SetNonSelectedDeviceTextColor()
    {
    }

    void ControlsOptionsRefresh()
    {
        controlsText.text = LanguageManager.GetText("Controls");
        actionsText.text = LanguageManager.GetText("Actions");
        keyboardText.text = LanguageManager.GetText("Keyboard");
        for (int i = 0; i < menu.Length; i++)
        {
            switch (menu[i].text[0].name)
            {
                case "MoveLeftText":
                    menu[i].text[0].text = LanguageManager.GetText("MoveLeft");
                    break;
                case "MoveRightText":
                    menu[i].text[0].text = LanguageManager.GetText("MoveRight");
                    break;
                case "MoveUpText":
                    menu[i].text[0].text = LanguageManager.GetText("MoveUp");
                    break;
                case "MoveDownText":
                    menu[i].text[0].text = LanguageManager.GetText("MoveDown");
                    break;
                case "JumpText":
                    menu[i].text[0].text = LanguageManager.GetText("Jump");
                    break;
                case "AttackText":
                    menu[i].text[0].text = LanguageManager.GetText("Attack");
                    break;
                case "ResetToDefaultText":
                    menu[i].text[0].text = LanguageManager.GetText("ResetToDefault");
                    break;
            }
        }
    }

    void KeyRefresh()
    {
        for (int i = 0; i < menu.Length; i++)
        {
            switch (menu[i].text[0].name)
            {
                case "MoveLeftText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.MoveLeft);
                    break;
                case "MoveRightText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.MoveRight);
                    break;
                case "MoveUpText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.MoveUp);
                    break;
                case "MoveDownText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.MoveDown);
                    break;
                case "JumpText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.Jump);
                    break;
                case "AttackText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.Attack);
                    break;
#if UNITY_EDITOR
                case "ResetToDefaultText":
                    break;
                default:
                    Debug.LogError(menu[i].text[0].name + "은 잘못된 이름입니다");
                    break;
#endif
            }
        }
    }



    void ReturnToOptionsMenuScreen()
    {
        gameObject.SetActive(false);
        optionsMenuScreen.SetActive(true);
    }
}
