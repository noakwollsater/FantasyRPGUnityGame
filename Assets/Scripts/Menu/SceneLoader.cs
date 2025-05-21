using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private CanvasGroup fadeGroup;
    private string targetScene;

    public static string sceneToLoad;

    private void Start()
    {
        targetScene = sceneToLoad;
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        yield return StartCoroutine(FadeIn());

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        progressBar.value = 1f;

        operation.allowSceneActivation = true; // 👈 flytta detta upp!

        // Nu tillåts scenen att bytas
        yield return null; // 👈 säkerställ att scenen hinner bytas innan fade

        yield return StartCoroutine(FadeOut()); // 👈 fade in i nästa scen istället (om CanvasGroup ligger kvar via DontDestroyOnLoad)

    }

    private IEnumerator FadeIn()
    {
        if (fadeGroup == null) yield break;

        fadeGroup.alpha = 1f;
        while (fadeGroup.alpha > 0)
        {
            fadeGroup.alpha -= Time.deltaTime * 1.5f;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        if (fadeGroup == null) yield break;

        while (fadeGroup.alpha < 1f)
        {
            fadeGroup.alpha += Time.deltaTime * 1.5f;
            yield return null;
        }
    }
}
