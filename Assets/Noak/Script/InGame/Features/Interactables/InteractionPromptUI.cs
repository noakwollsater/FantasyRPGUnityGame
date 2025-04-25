using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    public GameObject promptRoot;
    public TMP_Text promptText;

    public void Show(string prompt)
    {
        promptRoot.SetActive(true);
        promptText.text = prompt;
    }

    public void Hide()
    {
        promptRoot.SetActive(false);
    }
}
