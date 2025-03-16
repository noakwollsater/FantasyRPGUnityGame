using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceSelectionUI : MonoBehaviour
{
    public CharacterCreation characterCreation; // Reference to CharacterCreation script

    public static string SelectedRace { get; private set; } = "Human"; // Default race
    public static RaceSelectionUI Instance;

    [Header("Race Selection Buttons")]
    public Button humanButton, goblinButton, orcButton, apeButton, darkElfButton, drakoniteButton, dwarfButton, elfButton, hobbitButton, lynxButton;

    [Header("UI Elements for Displaying Stats")]
    public TextMeshProUGUI raceNameText;
    public TextMeshProUGUI descriptionText;

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
    }
}
