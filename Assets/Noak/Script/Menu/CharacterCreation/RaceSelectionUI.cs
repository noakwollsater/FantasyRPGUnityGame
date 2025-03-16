using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Import TextMeshPro for UI

public class RaceSelectionUI : MonoBehaviour
{
    public static string SelectedRace { get; private set; } = "Human"; // Default race
    public static RaceSelectionUI Instance;

    [Header("Race Selection Buttons")]
    public Button humanButton, goblinButton, orcButton, apeButton, darkElfButton, drakoniteButton, dwarfButton, elfButton, hobbitButton, lynxButton;

    [Header("UI Elements for Displaying Stats")]
    public TextMeshProUGUI raceNameText;
    public TextMeshProUGUI descriptionText; // 📝 New description field

    public TextMeshProUGUI strengthText, dexterityText, constitutionText, intelligenceText, wisdomText, charismaText;

    void Awake()
    {
        Instance = this;
        SelectedRace = "Human"; // Force reset to Human on restart
    }


    void Start()
    {
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

    public void UpdateRaceStats(string raceName, AttributeSet modifiedStats = null, AttributeSet classBonuses = null)
    {
        Race selectedRace = RaceDatabase.Instance.GetRace(raceName);
        if (selectedRace == null)
        {
            Debug.LogError($"Race '{raceName}' not found in database!");
            return;
        }

        raceNameText.text = selectedRace.RaceName;
        descriptionText.text = selectedRace.Description;

        if (modifiedStats != null)
        {
            // Ta bort både raceBonus och classBonus från modifierade stats innan vi visar dem
            int strength = modifiedStats.Strength - selectedRace.RacialBonuses.Strength;
            int dexterity = modifiedStats.Dexterity - selectedRace.RacialBonuses.Dexterity;
            int constitution = modifiedStats.Constitution - selectedRace.RacialBonuses.Constitution;
            int intelligence = modifiedStats.Intelligence - selectedRace.RacialBonuses.Intelligence;
            int wisdom = modifiedStats.Wisdom - selectedRace.RacialBonuses.Wisdom;
            int charisma = modifiedStats.Charisma - selectedRace.RacialBonuses.Charisma;

            // Lägg till klassbonus endast i parentesen, utan att ändra basvärdet
            strengthText.text = $"{strength} (+{selectedRace.RacialBonuses.Strength}, +{classBonuses?.Strength ?? 0})";
            dexterityText.text = $"{dexterity} (+{selectedRace.RacialBonuses.Dexterity}, +{classBonuses?.Dexterity ?? 0})";
            constitutionText.text = $"{constitution} (+{selectedRace.RacialBonuses.Constitution}, +{classBonuses?.Constitution ?? 0})";
            intelligenceText.text = $"{intelligence} (+{selectedRace.RacialBonuses.Intelligence}, +{classBonuses?.Intelligence ?? 0})";
            wisdomText.text = $"{wisdom} (+{selectedRace.RacialBonuses.Wisdom}, +{classBonuses?.Wisdom ?? 0})";
            charismaText.text = $"{charisma} (+{selectedRace.RacialBonuses.Charisma}, +{classBonuses?.Charisma ?? 0})";
        }
        else
        {
            // Visa bara grundvärden från rasen
            strengthText.text = $"{selectedRace.BaseAttributes.Strength} (+{selectedRace.RacialBonuses.Strength})";
            dexterityText.text = $"{selectedRace.BaseAttributes.Dexterity} (+{selectedRace.RacialBonuses.Dexterity})";
            constitutionText.text = $"{selectedRace.BaseAttributes.Constitution} (+{selectedRace.RacialBonuses.Constitution})";
            intelligenceText.text = $"{selectedRace.BaseAttributes.Intelligence} (+{selectedRace.RacialBonuses.Intelligence})";
            wisdomText.text = $"{selectedRace.BaseAttributes.Wisdom} (+{selectedRace.RacialBonuses.Wisdom})";
            charismaText.text = $"{selectedRace.BaseAttributes.Charisma} (+{selectedRace.RacialBonuses.Charisma})";
        }
    }
}
