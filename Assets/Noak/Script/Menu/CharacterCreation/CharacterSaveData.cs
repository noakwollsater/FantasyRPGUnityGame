using System.Collections.Generic;

[System.Serializable]
public class CharacterSaveData
{
    public string firstName;
    public string lastName;
    public string age;
    public string race;
    public string className;

    public int level = 1;
    public float experience = 0;
    public float experienceToNextLevel = 100;

    public string title = "Nobody";
    public string alignment = "Neutral good";

    public int bank = 0;
    public int gold = 0;
    public int silver = 0;
    public int copper = 0;

    public List<string> completedQuests = new();

    public float muscle;
    public float skinny;
    public float fat;
    public float genderBlend; // 0 = Female, 100 = Male

    public AttributeSet finalAttributes;

    public Dictionary<string, string> selectedParts = new();

    public string background;
    public List<string> backgroundSkills = new();
    public Dictionary<string, string> selectedColors = new();
}
