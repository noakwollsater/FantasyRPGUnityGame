using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.FantasyKingdom;

public class RaceSelectionUI : MonoBehaviour
{
    public CharacterCreation characterCreation; // Reference to CharacterCreation script

    public static string SelectedRace { get; private set; } = "Human"; // Default race
    public static RaceSelectionUI Instance;

    [Header("Race Selection Buttons")]
    public Button humanButton, goblinButton, orcButton, apeButton, darkElfButton, drakoniteButton, dwarfButton, elfButton, hobbitButton, lynxButton;

    [Header("UI Elements for Displaying Stats")]
    [SerializeField] private TextMeshProUGUI raceNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public TextMeshProUGUI strengthText, dexterityText, constitutionText, intelligenceText, wisdomText, charismaText;

    private AttributeSet classBonuses = new AttributeSet(0, 0, 0, 0, 0, 0);
    private AttributeSet modifiedAttributes;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SelectedRace = "Human"; // Force reset to Human on restart
    }

    void Start()
    {
        // Assign button listeners
        humanButton.onClick.AddListener(() => SelectRace("Human"));
        goblinButton.onClick.AddListener(() => SelectRace("Goblin"));
        orcButton.onClick.AddListener(() => SelectRace("Orc"));
        apeButton.onClick.AddListener(() => SelectRace("Ape"));
        darkElfButton.onClick.AddListener(() => SelectRace("Dark Elf"));
        drakoniteButton.onClick.AddListener(() => SelectRace("Drakonite"));
        dwarfButton.onClick.AddListener(() => SelectRace("Dwarf"));
        elfButton.onClick.AddListener(() => SelectRace("Elf"));
        hobbitButton.onClick.AddListener(() => SelectRace("Hobbit"));
        lynxButton.onClick.AddListener(() => SelectRace("Lynx"));

        UpdateRaceStats(SelectedRace);
    }

    void SelectRace(string raceName)
    {
        SelectedRace = raceName;
        Debug.Log($"Selected Race: {raceName}");
        UpdateRaceStats(raceName);
    }

    public void UpdateClassBonuses(int strBonus, int dexBonus, int conBonus, int intBonus, int wisBonus, int chaBonus)
    {
        classBonuses = new AttributeSet(strBonus, dexBonus, conBonus, intBonus, wisBonus, chaBonus);
    }

    public AttributeSet GetModifiedStats()
    {
        return modifiedAttributes;
    }

    public void UpdateRaceStats(string raceName, AttributeSet modifiedStats = null)
    {
        Race selectedRace = RaceDatabase.Instance.GetRace(raceName);
        if (selectedRace == null)
        {
            Debug.LogError($"Race '{raceName}' not found in database!");
            return;
        }

        raceNameText.text = selectedRace.RaceName;
        descriptionText.text = selectedRace.Description;

        modifiedAttributes = modifiedStats;

        int classStr = classBonuses.Strength;
        int classDex = classBonuses.Dexterity;
        int classCon = classBonuses.Constitution;
        int classInt = classBonuses.Intelligence;
        int classWis = classBonuses.Wisdom;
        int classCha = classBonuses.Charisma;

        if (modifiedStats != null)
        {
            int strength = modifiedStats.Strength - selectedRace.RacialBonuses.Strength;
            int dexterity = modifiedStats.Dexterity - selectedRace.RacialBonuses.Dexterity;
            int constitution = modifiedStats.Constitution - selectedRace.RacialBonuses.Constitution;
            int intelligence = modifiedStats.Intelligence - selectedRace.RacialBonuses.Intelligence;
            int wisdom = modifiedStats.Wisdom - selectedRace.RacialBonuses.Wisdom;
            int charisma = modifiedStats.Charisma - selectedRace.RacialBonuses.Charisma;

            strengthText.text = $"{strength} (+{selectedRace.RacialBonuses.Strength}, +{classStr})";
            dexterityText.text = $"{dexterity} (+{selectedRace.RacialBonuses.Dexterity}, +{classDex})";
            constitutionText.text = $"{constitution} (+{selectedRace.RacialBonuses.Constitution}, +{classCon})";
            intelligenceText.text = $"{intelligence} (+{selectedRace.RacialBonuses.Intelligence}, +{classInt})";
            wisdomText.text = $"{wisdom} (+{selectedRace.RacialBonuses.Wisdom}, +{classWis})";
            charismaText.text = $"{charisma} (+{selectedRace.RacialBonuses.Charisma}, +{classCha})";
        }
        else
        {
            strengthText.text = $"{selectedRace.BaseAttributes.Strength} (+{selectedRace.RacialBonuses.Strength}, +{classStr})";
            dexterityText.text = $"{selectedRace.BaseAttributes.Dexterity} (+{selectedRace.RacialBonuses.Dexterity}, +{classDex})";
            constitutionText.text = $"{selectedRace.BaseAttributes.Constitution} (+{selectedRace.RacialBonuses.Constitution}, +{classCon})";
            intelligenceText.text = $"{selectedRace.BaseAttributes.Intelligence} (+{selectedRace.RacialBonuses.Intelligence}, +{classInt})";
            wisdomText.text = $"{selectedRace.BaseAttributes.Wisdom} (+{selectedRace.RacialBonuses.Wisdom}, +{classWis})";
            charismaText.text = $"{selectedRace.BaseAttributes.Charisma} (+{selectedRace.RacialBonuses.Charisma}, +{classCha})";
        }

        modifiedAttributes = new AttributeSet(
            selectedRace.BaseAttributes.Strength + selectedRace.RacialBonuses.Strength,
            selectedRace.BaseAttributes.Dexterity + selectedRace.RacialBonuses.Dexterity,
            selectedRace.BaseAttributes.Constitution + selectedRace.RacialBonuses.Constitution,
            selectedRace.BaseAttributes.Intelligence + selectedRace.RacialBonuses.Intelligence,
            selectedRace.BaseAttributes.Wisdom + selectedRace.RacialBonuses.Wisdom,
            selectedRace.BaseAttributes.Charisma + selectedRace.RacialBonuses.Charisma
        );


        GetFinalAttributes();    
    }

    public static AttributeSet GetFinalAttributes()
    {
        if (Instance == null)
        {
            Debug.LogWarning("⚠️ GetFinalAttributes: Instance is null.");
            return new AttributeSet(0, 0, 0, 0, 0, 0);
        }

        Race race = RaceDatabase.Instance.GetRace(SelectedRace);
        AttributeSet baseStats = new AttributeSet(
            race.BaseAttributes.Strength + race.RacialBonuses.Strength,
            race.BaseAttributes.Dexterity + race.RacialBonuses.Dexterity,
            race.BaseAttributes.Constitution + race.RacialBonuses.Constitution,
            race.BaseAttributes.Intelligence + race.RacialBonuses.Intelligence,
            race.BaseAttributes.Wisdom + race.RacialBonuses.Wisdom,
            race.BaseAttributes.Charisma + race.RacialBonuses.Charisma
        );

        float muscle = DictionaryLibrary.Instance.MusclesBlendValue / 100f;
        float skinny = DictionaryLibrary.Instance.BodySizeSkinnyBlendValue / 100f;
        float fat = DictionaryLibrary.Instance.BodySizeHeavyBlendValue / 100f;

        baseStats.ApplyBodyCompositionModifiers(muscle, skinny, fat);

        AttributeSet finalStats = new AttributeSet(
            baseStats.Strength + Instance.classBonuses.Strength,
            baseStats.Dexterity + Instance.classBonuses.Dexterity,
            baseStats.Constitution + Instance.classBonuses.Constitution,
            baseStats.Intelligence + Instance.classBonuses.Intelligence,
            baseStats.Wisdom + Instance.classBonuses.Wisdom,
            baseStats.Charisma + Instance.classBonuses.Charisma
        );

        return finalStats;
    }

    public static ExtendedStats GetFinalStats()
    {
        if (Instance == null)
        {
            Debug.LogWarning("⚠️ GetFinalStats: Instance is null.");
            return null;
        }

        Race race = RaceDatabase.Instance.GetRace(SelectedRace);

        return new ExtendedStats(
            race.ExtendedStats.HP,
            race.ExtendedStats.Stamina,
            race.ExtendedStats.Hunger,
            race.ExtendedStats.Thirst,
            race.ExtendedStats.Speed,
            race.ExtendedStats.Armor,
            race.ExtendedStats.Mana
        );
    }

    public static AttributeSet RecalculateAttributes(string raceName, AttributeSet classBonuses)
    {
        Race selectedRace = RaceDatabase.Instance.GetRace(raceName);

        // Base + race bonuses
        AttributeSet combined = new AttributeSet(
            selectedRace.BaseAttributes.Strength + selectedRace.RacialBonuses.Strength,
            selectedRace.BaseAttributes.Dexterity + selectedRace.RacialBonuses.Dexterity,
            selectedRace.BaseAttributes.Constitution + selectedRace.RacialBonuses.Constitution,
            selectedRace.BaseAttributes.Intelligence + selectedRace.RacialBonuses.Intelligence,
            selectedRace.BaseAttributes.Wisdom + selectedRace.RacialBonuses.Wisdom,
            selectedRace.BaseAttributes.Charisma + selectedRace.RacialBonuses.Charisma
        );

        // Apply body composition again
        float muscle = DictionaryLibrary.Instance.MusclesBlendValue / 100f;
        float skinny = DictionaryLibrary.Instance.BodySizeSkinnyBlendValue / 100f;
        float fat = DictionaryLibrary.Instance.BodySizeHeavyBlendValue / 100f;

        combined.ApplyBodyCompositionModifiers(muscle, skinny, fat);

        // Return result BEFORE class bonuses; they'll be displayed separately in UI
        return combined;
    }

}
