using UnityEngine;
using TMPro;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    public int levelPoints = 15;
    public TMP_Text levelPointsText;

    private void OnEnable()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        levelPointsText.text = levelPoints.ToString();
    }

    public bool HasEnoughPoints(int cost) => levelPoints >= cost;

    public void SpendPoints(int amount)
    {
        levelPoints -= amount;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (levelPointsText != null)
            levelPointsText.text = levelPoints.ToString();
    }
}
