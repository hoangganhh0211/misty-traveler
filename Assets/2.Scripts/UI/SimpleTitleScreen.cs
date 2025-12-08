using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SimpleTitleScreen : MonoBehaviour
{
    public TextMeshProUGUI startText;
    public TextMeshProUGUI exitText;

    private int selectedIndex = 0;
    private readonly Color selectedColor = new Color32(255, 236, 214, 255);
    private readonly Color normalColor = new Color32(255, 170, 94, 255);

    void Start()
    {
        UpdateMenuColors();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedIndex = 0;
            UpdateMenuColors();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selectedIndex = 1;
            UpdateMenuColors();
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            SelectMenu();
        }
    }

    void UpdateMenuColors()
    {
        if (startText != null)
            startText.color = selectedIndex == 0 ? selectedColor : normalColor;
        if (exitText != null)
            exitText.color = selectedIndex == 1 ? selectedColor : normalColor;
    }

    void SelectMenu()
    {
        if (selectedIndex == 0)
        {
            StartGame();
        }
        else if (selectedIndex == 1)
        {
            ExitGame();
        }
    }

    void StartGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GameLoad(1, true);
        }
        else
        {
            if (SceneTransition.instance != null)
                SceneTransition.instance.LoadScene("OldMachineRoom_A");
            else
                SceneManager.LoadScene("OldMachineRoom_A");
        }
    }

    void ExitGame()
    {
        OptionsData.OptionsSave();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
