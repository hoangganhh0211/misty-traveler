using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Text;

namespace MenuUI
{

    [System.Serializable]
    public class Manual
    {
        public string key;
        public GameInputManager.MenuControl menuControl;
    }

    [System.Serializable]
    public class Menu
    {
        public TextMeshProUGUI[] text;
        public Manual[] manual;
        public UnityEvent menuSelectEvent;
    }

    public static class MenuUIController
    {
        public static readonly Color selectColor = new Color32(255, 236, 214, 255);
        public static readonly Color notSelectColor = new Color32(255, 170, 94, 255);

        public static void MenuRefresh(Menu[] menu, ref int currentMenuIndex, TextMeshProUGUI manualText = null)
        {
            if (currentMenuIndex >= menu.Length)
            {
                currentMenuIndex = 0;
            }
            else if (currentMenuIndex < 0)
            {
                currentMenuIndex = menu.Length - 1;
            }

            AllMenuTextColorInit(menu);
            SelectMenuTextColorChange(menu[currentMenuIndex]);

            if (manualText != null)
            {
                SetMenualText(menu[currentMenuIndex], manualText);
            }
        }

        public static void MenuRefresh(List<Menu> menu, ref int currentMenuIndex, TextMeshProUGUI manualText = null)
        {
            if (currentMenuIndex >= menu.Count)
            {
                currentMenuIndex = 0;
            }
            else if (currentMenuIndex < 0)
            {
                currentMenuIndex = menu.Count - 1;
            }

            AllMenuTextColorInit(menu);
            SelectMenuTextColorChange(menu[currentMenuIndex]);

            if (manualText != null)
            {
                SetMenualText(menu[currentMenuIndex], manualText);
            }
        }

        public static void AllMenuTextColorInit(Menu[] menu)
        {
            for (int i = 0; i < menu.Length; i++)
            {
                if (menu[i].text != null)
                {
                    for(int j = 0; j < menu[i].text.Length; j++)
                    {
                        if (menu[i].text[j] != null)
                            menu[i].text[j].color = notSelectColor;
                    }
                }
            }
        }

        public static void AllMenuTextColorInit(List<Menu> menu)
        {
            for (int i = 0; i < menu.Count; i++)
            {
                if (menu[i].text != null)
                {
                    for (int j = 0; j < menu[i].text.Length; j++)
                    {
                        if (menu[i].text[j] != null)
                            menu[i].text[j].color = notSelectColor;
                    }
                }
            }
        }

        public static void AllMenuTextHide(Menu[] menu)
        {
            for (int i = 0; i < menu.Length; i++)
            {
                if (menu[i].text != null)
                {
                    for (int j = 0; j < menu[i].text.Length; j++)
                    {
                        if (menu[i].text[j] != null)
                            menu[i].text[j].color = new Color32(0, 0, 0, 0);
                    }
                }
            }
        }

        public static void AllMenuTextHide(List<Menu> menu)
        {
            for (int i = 0; i < menu.Count; i++)
            {
                if (menu[i].text != null)
                {
                    for (int j = 0; j < menu[i].text.Length; j++)
                    {
                        if (menu[i].text[j] != null)
                            menu[i].text[j].color = new Color32(0, 0, 0, 0);
                    }
                }
            }
        }

        public static void SelectMenuTextColorChange(Menu menu)
        {
            if (menu.text != null)
            {
                for (int j = 0; j < menu.text.Length; j++)
                {
                    if (menu.text[j] != null)
                        menu.text[j].color = selectColor;
                }
            }
        }

        public static void SelectMenuTextHide(Menu menu)
        {
            if (menu.text != null)
            {
                for (int j = 0; j < menu.text.Length; j++)
                {
                    if (menu.text[j] != null)
                        menu.text[j].color = new Color(0, 0, 0, 0);
                }
            }
        }

        public static void SetMenualText(Menu menu, TextMeshProUGUI manualText)
        {

            StringBuilder manual = new StringBuilder();
            for (int i = 0; i < menu.manual.Length; i++)
            {
                string key = LanguageManager.GetText(menu.manual[i].key);
                string input = GameInputManager.usingController ? GameInputManager.MenuControlToButtonText(menu.manual[i].menuControl)
                                                                : GameInputManager.MenuControlToKeyText(menu.manual[i].menuControl);
                manual.AppendFormat("{0} [ <color=#ffaa5e>{1}</color> ]", key, input);
                if (i < menu.manual.Length - 1)
                {
                    manual.Append(" ");
                }
            }
            manualText.text = manual.ToString();
        }
    }
}