using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAddaptive : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadSceneAsync("OutdoorsScene", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
    }
}