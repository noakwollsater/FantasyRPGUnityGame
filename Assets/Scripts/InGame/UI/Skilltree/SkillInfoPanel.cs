using UnityEngine;
using TMPro;

public class SkillInfoPanel : MonoBehaviour
{
    public static SkillInfoPanel Instance;

    public GameObject panelRoot;
    public TMP_Text skillNameText;
    public TMP_Text skillDescriptionText;
    public TMP_Text skillCostText;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string skillName, string description, int cost)
    {
        skillNameText.text = skillName;
        skillDescriptionText.text = description;
        skillCostText.text = $"Kostar: {cost}";

        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }
}
