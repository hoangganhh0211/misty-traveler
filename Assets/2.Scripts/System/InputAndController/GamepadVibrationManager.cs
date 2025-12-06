using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadVibrationManager : MonoBehaviour
{

    public static GamepadVibrationManager instance = null;
    Coroutine _gamepadRumble = null;

    void Awake()
    {

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GamepadRumbleStart(float left, float right, float duration)
    {

        if(!GameInputManager.usingController) return;

        if (_gamepadRumble != null)
        {
            StopCoroutine(_gamepadRumble);
            _gamepadRumble = null;
        }

        left = left * AccessibilitySettingsManager.gamepadVibration;
        right = right * AccessibilitySettingsManager.gamepadVibration;
        _gamepadRumble = StartCoroutine(GamepadRumble(left, right, duration));
    }

    public void GamepadRumbleStart(float intensity, float duration)
    {

        if (!GameInputManager.usingController) return;

        if (_gamepadRumble != null)
        {
            StopCoroutine(_gamepadRumble);
            _gamepadRumble = null;
        }

        intensity = intensity * AccessibilitySettingsManager.gamepadVibration;
        _gamepadRumble = StartCoroutine(GamepadRumble(intensity * 0.3f, intensity, duration));
    }

    public void GamepadRumbleStop()
    {

        if (!GameInputManager.usingController) return;

        if (_gamepadRumble != null)
        {
            StopCoroutine(_gamepadRumble);
            InputSystem.ResetHaptics();
            _gamepadRumble = null;
        }
    }

    IEnumerator GamepadRumble(float left, float right, float duration)
    {

        Gamepad.current.SetMotorSpeeds(left, right);

        yield return YieldInstructionCache.WaitForSecondsRealtime(duration);

        InputSystem.ResetHaptics();
        _gamepadRumble = null;
    }
}
