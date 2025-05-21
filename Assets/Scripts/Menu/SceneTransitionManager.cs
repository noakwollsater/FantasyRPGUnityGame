using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keep across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string targetScene)
    {
        SceneLoader.sceneToLoad = targetScene;
        SceneManager.LoadScene("LoadingScene");
    }
}
