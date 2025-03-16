using UnityEngine;
using TMPro;

public class ClassSelectionUI : MonoBehaviour
{
    public static string SelectedClass { get; private set; } = "Barbarian"; // Default class

    [Header("UI Elements for Displaying Class Info")]
    public TextMeshProUGUI classNameText;
    public TextMeshProUGUI descriptionText;

    void Start()
    {
        UpdateClassInfo(SelectedClass);
    }

    // 📌 One method for selecting a class
    public void SelectClass(string className)
    {
        SelectedClass = className;
        UpdateClassInfo(className);

        ApplyClassBonuses();
    }

    // 📌 Assign these methods to buttons in Unity Inspector
    public void ChooseBarbarian() => SelectClass("Barbarian");
    public void ChooseBard() => SelectClass("Bard");
    public void ChooseCleric() => SelectClass("Cleric");
    public void ChooseDruid() => SelectClass("Druid");
    public void ChooseFighter() => SelectClass("Fighter");
    public void ChooseMonk() => SelectClass("Monk");
    public void ChoosePaladin() => SelectClass("Paladin");
    public void ChooseRanger() => SelectClass("Ranger");
    public void ChooseRogue() => SelectClass("Rogue");
    public void ChooseSorcerer() => SelectClass("Sorcerer");
    public void ChooseWarlock() => SelectClass("Warlock");
    public void ChooseWizard() => SelectClass("Wizard");

    private void UpdateClassInfo(string className)
    {
        Class selectedClass = ClassDatabase.Instance.GetClass(className);
        if (selectedClass == null)
        {
            Debug.LogError($"Class '{className}' not found in database!");
            return;
        }

        classNameText.text = selectedClass.ClassName;
        descriptionText.text = selectedClass.Description;
    }

    private void ApplyClassBonuses()
    {
        if (RaceSelectionUI.SelectedRace == null)
        {
            Debug.LogError("Race is not selected!");
            return;
        }

        Race selectedRace = RaceDatabase.Instance.GetRace(RaceSelectionUI.SelectedRace);
        Class selectedClass = ClassDatabase.Instance.GetClass(SelectedClass);

        if (selectedRace == null || selectedClass == null)
        {
            Debug.LogError($"Race or Class not found!");
            return;
        }

        // 🛠 Hämta basattributen utan att generera nya
        AttributeSet baseAttributes = new AttributeSet(
            selectedRace.BaseAttributes.Strength,
            selectedRace.BaseAttributes.Dexterity,
            selectedRace.BaseAttributes.Constitution,
            selectedRace.BaseAttributes.Intelligence,
            selectedRace.BaseAttributes.Wisdom,
            selectedRace.BaseAttributes.Charisma
        );

        // 🛠 Applicera kroppskomposition (utan att skriva över grundvärdena)
        baseAttributes.ApplyBodyCompositionModifiers(
            DefaultBodyComposition.DefaultMuscle,
            DefaultBodyComposition.DefaultSkinny,
            DefaultBodyComposition.DefaultFat
        );

        // ✅ Skicka klassbonusarna som en separat parameter
        RaceSelectionUI.Instance.UpdateRaceStats(selectedRace.RaceName, baseAttributes, selectedClass.ClassBonuses);
    }
}
