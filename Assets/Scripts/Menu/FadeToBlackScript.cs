using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeToBlackScript : MonoBehaviour
{
    [SerializeField] private GameObject fadeToBlack;
    [SerializeField] private Button[] buttonToUse;

    void Start()
    {
        fadeToBlack.SetActive(false); // Hide the fade panel at the start
        findButns();
    }

    private void findButns()
    {
        // Find all buttons with "Screen" in the name
        buttonToUse = FindObjectsOfType<Button>().Where(b => b.name.Contains("Screen")).ToArray();

        // Add the fade trigger to each button
        foreach (Button button in buttonToUse)
        {
            button.onClick.AddListener(() => StartCoroutine(ToggleFade()));
        }
    }

    private IEnumerator ToggleFade()
    {
        fadeToBlack.SetActive(true);
        findButns();
        yield return new WaitForSeconds(0.5f); // Wait for 4 seconds
        fadeToBlack.SetActive(false);
    }
}
