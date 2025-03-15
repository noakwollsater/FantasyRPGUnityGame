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
        Instance = this;  // Ensure singleton reference
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

    public void UpdateRaceStats(string raceName, AttributeSet updatedStats = null)
    {
        Race selectedRace = RaceDatabase.Instance.GetRace(raceName);
        if (selectedRace == null)
        {
            Debug.LogError($"Race '{raceName}' not found in database!");
            return;
        }

        raceNameText.text = selectedRace.RaceName;
        descriptionText.text = selectedRace.Description;

        if (updatedStats != null)
        {
            strengthText.text = $"STR: {updatedStats.Strength}";
            dexterityText.text = $"DEX: {updatedStats.Dexterity}";
            constitutionText.text = $"CON: {updatedStats.Constitution}";
            intelligenceText.text = $"INT: {updatedStats.Intelligence}";
            wisdomText.text = $"WIS: {updatedStats.Wisdom}";
            charismaText.text = $"CHA: {updatedStats.Charisma}";
        }
        else
        {
            strengthText.text = $"STR: {selectedRace.BaseAttributes.Strength} ( +{selectedRace.RacialBonuses.Strength} )";
            dexterityText.text = $"DEX: {selectedRace.BaseAttributes.Dexterity} ( +{selectedRace.RacialBonuses.Dexterity} )";
            constitutionText.text = $"CON: {selectedRace.BaseAttributes.Constitution} ( +{selectedRace.RacialBonuses.Constitution} )";
            intelligenceText.text = $"INT: {selectedRace.BaseAttributes.Intelligence} ( +{selectedRace.RacialBonuses.Intelligence} )";
            wisdomText.text = $"WIS: {selectedRace.BaseAttributes.Wisdom} ( +{selectedRace.RacialBonuses.Wisdom} )";
            charismaText.text = $"CHA: {selectedRace.BaseAttributes.Charisma} ( +{selectedRace.RacialBonuses.Charisma} )";
        }
    }

}
