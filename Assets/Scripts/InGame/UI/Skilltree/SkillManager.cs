using UnityEngine;
using TMPro;
using Unity.FantasyKingdom;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    LoadCharacterData loadCharacterData;
    public int levelPoints;
    public TMP_Text levelPointsText;

    private void OnEnable()
    {
        if (loadCharacterData == null)
            loadCharacterData = GameObject.FindGameObjectWithTag("HUD").GetComponent<LoadCharacterData>();
        if (Instance == null) Instance = this;

        levelPoints = loadCharacterData.levelPoints;
        levelPointsText.text = levelPoints.ToString();
    }

    public bool HasEnoughPoints(int cost) => levelPoints >= cost;

    public void SpendPoints(int amount)
    {
        levelPoints -= amount;
        UpdateUI();
        RemovePoints(amount);
    }

    public void UpdateUI()
    {
        if (levelPointsText != null)
            levelPointsText.text = levelPoints.ToString();
    }

    public void RemovePoints(int amount)
    {
        loadCharacterData.levelPoints -= amount;
    }
}
