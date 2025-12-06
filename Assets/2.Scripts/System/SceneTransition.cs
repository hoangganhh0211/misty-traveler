using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{

    public static SceneTransition instance = null;

    const float FadeSpeed = 0.08f;

    bool isFadingIn;

    Image _fadeEffectImage;
    Color _fadeEffectColor;

    void Awake()
    {
        instance = this;

        _fadeEffectImage = GetComponent<Image>();
        _fadeEffectColor = _fadeEffectImage.color;

        StartCoroutine(FadeIn());
    }

    public void LoadScene(string nextScene)
    {
        StartCoroutine(FadeEffect(nextScene));
    }

    IEnumerator FadeEffect(string nextScene)
    {
        yield return StartCoroutine(FadeOut());
        LoadingSceneManager.LoadScene(nextScene);
    }

    IEnumerator FadeIn()
    {

        isFadingIn = true;

        float r = _fadeEffectColor.r;
        float g = _fadeEffectColor.g;
        float b = _fadeEffectColor.b;

        float alpha = 1;
        _fadeEffectImage.color = new Color(r, g, b, alpha);

        yield return null;

        while(alpha > 0f)
        {
            alpha -= FadeSpeed;
            _fadeEffectImage.color = new Color(r, g, b, alpha);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        isFadingIn = false;
    }

    IEnumerator FadeOut()
    {
        float r = _fadeEffectColor.r;
        float g = _fadeEffectColor.g;
        float b = _fadeEffectColor.b;

        while(isFadingIn)
        {
            yield return null;
        }

        float alpha = 0;
        while (alpha < 1f)
        {
            alpha += FadeSpeed;
            _fadeEffectImage.color = new Color(r, g, b, alpha);

            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        yield return YieldInstructionCache.WaitForSecondsRealtime(0.07f);
    }
}
