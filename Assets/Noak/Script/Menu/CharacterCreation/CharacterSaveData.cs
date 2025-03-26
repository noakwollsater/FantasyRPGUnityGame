using System.Collections.Generic;

[System.Serializable]
public class CharacterSaveData
{
    public string firstName;
    public string lastName;
    public string age;
    public string race;
    public string className;

    public float muscle;
    public float skinny;
    public float fat;
    public float genderBlend; // 0 = Female, 100 = Male

    public AttributeSet finalAttributes;

    public Dictionary<string, string> selectedParts = new();

    public string background;
    public List<string> backgroundSkills = new();
}
