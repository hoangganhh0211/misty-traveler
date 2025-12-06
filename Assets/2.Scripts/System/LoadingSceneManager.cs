using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;

    void Start()
    {

        StartCoroutine(LoadSceneCoroutine());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
        System.GC.Collect();
    }

    IEnumerator LoadSceneCoroutine()
    {

        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        asyncOperation.allowSceneActivation = false;

        while(!asyncOperation.isDone)
        {
            yield return null;

            if(asyncOperation.progress >= 0.9f)
            {

                Time.timeScale = 1f;
                asyncOperation.allowSceneActivation = true;
            }
        }
    }
}