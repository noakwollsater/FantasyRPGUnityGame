using UnityEngine;
using TMPro;
using Unity.FantasyKingdom;

public class ClassSelectionUI : CharacterCreation
{
    public static string SelectedClass { get; private set; } = "Barbarian"; // Default class

    [Header("UI Elements for Displaying Class Info")]
    [SerializeField] private TextMeshProUGUI classNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private SummaryCreation summaryCreation;

    void Start()
    {
        UpdateClassInfo(SelectedClass);
    }

    // 📌 One method for selecting a class
    public void SelectClass(string className)
    {
        SelectedClass = className;
        UpdateClassInfo(className);
        _dictionaryLibrary.selectedClass = className;
        ApplyClassBonuses();
        AddClassSkills();
        summaryCreation.UpdateSummary();
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

        Class selectedClass = ClassDatabase.Instance.GetClass(SelectedClass);

        if (selectedClass == null)
        {
            Debug.LogError($"Class '{SelectedClass}' not found!");
            return;
        }

        // ✅ Sätt klassbonusarna
        RaceSelectionUI.Instance.UpdateClassBonuses(
            selectedClass.ClassBonuses.Strength,
            selectedClass.ClassBonuses.Dexterity,
            selectedClass.ClassBonuses.Constitution,
            selectedClass.ClassBonuses.Intelligence,
            selectedClass.ClassBonuses.Wisdom,
            selectedClass.ClassBonuses.Charisma
        );

        // 🛠️ Beräkna modifierade stats


        var modifiedStats = RaceSelectionUI.RecalculateAttributes(RaceSelectionUI.SelectedRace, selectedClass.ClassBonuses);
        RaceSelectionUI.Instance.UpdateClassBonuses(
            selectedClass.ClassBonuses.Strength,
            selectedClass.ClassBonuses.Dexterity,
            selectedClass.ClassBonuses.Constitution,
            selectedClass.ClassBonuses.Intelligence,
            selectedClass.ClassBonuses.Wisdom,
            selectedClass.ClassBonuses.Charisma
        );
        RaceSelectionUI.Instance.UpdateRaceStats(RaceSelectionUI.SelectedRace, modifiedStats);

    }

    private void AddClassSkills()
    {
        _dictionaryLibrary.classSkills.Clear(); // ✅ Rensa innan du lägger till nya

        Class selectedClass = ClassDatabase.Instance.GetClass(SelectedClass);

        if (selectedClass == null)
        {
            Debug.LogError($"Class '{SelectedClass}' not found!");
            return;
        }

        foreach (var skill in selectedClass.UniqueAbilities)
        {
            if (!_dictionaryLibrary.classSkills.Contains(skill))
            {
                _dictionaryLibrary.classSkills.Add(skill);
            }
        }
    }
}
