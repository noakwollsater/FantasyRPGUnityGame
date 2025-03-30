using UnityEngine;
using System.Collections;
using Synty.SidekickCharacters.Enums;
using System.Collections.Generic;
using static RaceSelectionUI;

[CreateAssetMenu(fileName = "NewDictionaryLibrary", menuName = "Custom/Dictionary Library")]
public class DictionaryLibrary : ScriptableObject
{
    public static DictionaryLibrary Instance;

    private void OnEnable()
    {
        Instance = this;
    }

    [System.Serializable]
    public class RaceAgeData
    {
        public string race;
        public int minAge;
        public int maxAge;
        public int defaultAge;
    }

    public Dictionary<CharacterPartType, int> _partIndexDictionary = new Dictionary<CharacterPartType, int>();
    public Dictionary<CharacterPartType, Dictionary<string, string>> _availablePartDictionary = new Dictionary<CharacterPartType, Dictionary<string, string>>();
    public Dictionary<CharacterPartType, Dictionary<string, string>> _partLibrary;
    public Dictionary<string, Dictionary<string, Color>> speciesColors = new Dictionary<string, Dictionary<string, Color>>
{
    { "Human", new Dictionary<string, Color>
        {
            { "Skin", new Color(0.96f, 0.80f, 0.69f) },
            { "Eyes", new Color(1.0f, 1.0f, 1.0f) },
            { "Hair", new Color(0.50f, 0.35f, 0.20f) },
            { "Fingernails", new Color(0.85f, 0.65f, 0.545f) } 
        }
    },
    { "Goblin", new Dictionary<string, Color>
        {
            { "Skin", new Color(0.533f, 0.510f, 0.298f) },
            { "Eyes", new Color(0.85f, 0.10f, 0.10f) },
            { "Hair", new Color(0.10f, 0.10f, 0.10f) },
            { "Fingernails", new Color(0.282f, 0.266f, 0.165f) }

        }
    },
    { "Orc", new Dictionary<string, Color>
        {
            { "Skin", new Color(0.30f, 0.40f, 0.20f) },
            { "Eyes", new Color(0.90f, 0.80f, 0.50f) }, 
            { "Hair", new Color(0.10f, 0.10f, 0.10f) } 
        }
    },
    { "Elf", new Dictionary<string, Color>
        {
            { "Skin", new Color(0.90f, 0.85f, 0.75f) },
            { "Eyes", new Color(1.0f, 1.0f, 1.0f) },
            { "Hair", new Color(1.0f, 1.0f, 0.85f) }   
        }
    },
    { "Dwarf", new Dictionary<string, Color>
        {
            { "Skin", new Color(0.85f, 0.70f, 0.60f) },
            { "Eyes", new Color(1.0f, 1.0f, 1.0f) },
            { "Hair", new Color(0.70f, 0.40f, 0.20f) }  
        }
    }
};
    public Dictionary<string, List<string>> bodyPartMappings = new Dictionary<string, List<string>>
    {
        { "Skin", new List<string> { "skin", "nose", "ear", "eyelids", "fingernails" } },
        { "Eyes", new List<string> { "eye outer" } },
        { "Hair", new List<string> { "hair", "facial hair", "eyebrow" } },
            { "Fingernails", new List<string> { "fingernail" } }
    };
    public Dictionary<CharacterPartType, HashSet<string>> AllowedParts = new()
    {
        { CharacterPartType.Torso, new HashSet<string> {
            "SK_HUMN_BASE_01_10TORS_HU01",
            "SK_GOBL_BASE_01_10TORS_GO01",
            "SK_VIKG_WARR_05_10TORS_HU01",
            "SK_APOC_OUTL_04_10TORS_HU01",
            "SK_PIRT_CAPT_09_10TORS_HU01",
            "SK_PIRT_CAPT_10_10TORS_HU01",
            "SK_GOBL_FIGT_06_10TORS_GO01"} },

        { CharacterPartType.ArmUpperLeft, new HashSet<string> {
            "SK_HUMN_BASE_01_11AUPL_HU01",
            "SK_GOBL_BASE_01_11AUPL_GO01",
            "SK_PIRT_CAPT_10_11AUPL_HU01",
            "SK_APOC_OUTL_03_11AUPL_HU01",
            "SK_APOC_OUTL_08_11AUPL_HU01",
            "SK_GOBL_FIGT_06_11AUPL_GO01"} },

        { CharacterPartType.ArmUpperRight, new HashSet<string> {
            "SK_HUMN_BASE_01_12AUPR_HU01",
            "SK_GOBL_BASE_01_12AUPR_GO01",
            "SK_PIRT_CAPT_10_12AUPR_HU01",
            "SK_APOC_OUTL_03_12AUPR_HU01",
            "SK_APOC_OUTL_08_12AUPR_HU01",
            "SK_GOBL_FIGT_06_12AUPR_GO01"} },

        { CharacterPartType.ArmLowerLeft, new HashSet<string> {
            "SK_HUMN_BASE_01_13ALWL_HU01",
            "SK_GOBL_BASE_01_13ALWL_GO01",
            "SK_PIRT_CAPT_10_13ALWL_HU01",
            "SK_APOC_OUTL_10_13ALWL_HU01"} },

        { CharacterPartType.ArmLowerRight, new HashSet<string> {
            "SK_HUMN_BASE_01_14ALWR_HU01",
            "SK_GOBL_BASE_01_14ALWR_GO01",
            "SK_PIRT_CAPT_10_14ALWR_HU01",
            "SK_APOC_OUTL_10_14ALWR_HU01" } },

        { CharacterPartType.HandLeft, new HashSet<string> {
            "SK_HUMN_BASE_01_15HNDL_HU01",
            "SK_GOBL_BASE_01_15HNDL_GO01",
            "SK_PIRT_CAPT_08_15HNDL_HU01",
            "SK_PIRT_CAPT_10_15HNDL_HU01"} },
        { CharacterPartType.HandRight, new HashSet<string> {
            "SK_HUMN_BASE_01_16HNDR_HU01",
            "SK_GOBL_BASE_01_16HNDR_GO01",
            "SK_PIRT_CAPT_08_16HNDR_HU01",
            "SK_PIRT_CAPT_10_16HNDR_HU01" } },

        { CharacterPartType.Hips, new HashSet<string> {
            "SK_HUMN_BASE_01_17HIPS_HU01",
            "SK_GOBL_BASE_01_17HIPS_GO01",
            "SK_APOC_OUTL_07_17HIPS_HU01",
            "SK_PIRT_CAPT_09_17HIPS_HU01", } },

        { CharacterPartType.LegLeft, new HashSet<string> {
            "SK_HUMN_BASE_01_18LEGL_HU01",
            "SK_GOBL_BASE_01_18LEGL_GO01",
            "SK_APOC_OUTL_08_18LEGL_HU01",
            "SK_PIRT_CAPT_06_18LEGL_HU01"
            }},
        { CharacterPartType.LegRight, new HashSet<string> {
            "SK_HUMN_BASE_01_19LEGR_HU01",
            "SK_GOBL_BASE_01_19LEGR_GO01",
            "SK_APOC_OUTL_08_19LEGR_HU01",
            "SK_PIRT_CAPT_06_19LEGR_HU01" } },

        { CharacterPartType.FootLeft, new HashSet<string> {
            "SK_HUMN_BASE_01_20FOTL_HU01",
            "SK_GOBL_BASE_01_20FOTL_GO01",
            "SK_APOC_OUTL_06_20FOTL_HU01" }},

        { CharacterPartType.FootRight, new HashSet<string> {
            "SK_HUMN_BASE_01_21FOTR_HU01",
            "SK_GOBL_BASE_01_21FOTR_GO01",
            "SK_APOC_OUTL_06_21FOTR_HU01" } },
    };
    public Dictionary<string, List<string>> skillMap = new Dictionary<string, List<string>>
{
    // Environment-related skills
    { "a poor district", new List<string> { "Stealth", "Sleight of Hand" } },
    { "a remote mountain village", new List<string> { "Nature", "Survival" } },
    { "a nomadic tribe", new List<string> { "Animal Handling", "Survival" } },
    { "a noble estate", new List<string> { "History", "Persuasion" } },
    { "a forgotten ruin city", new List<string> { "Investigation", "Arcana" } },
    { "a guildhall in a bustling city", new List<string> { "Insight", "Persuasion" } },

    // Attitude-related skills
    { "a cynical loner", new List<string> { "Perception", "Deception" } },
    { "a driven avenger", new List<string> { "Intimidation", "Athletics" } },
    { "a protective leader", new List<string> { "Persuasion", "Medicine" } },
    { "a curious explorer", new List<string> { "Investigation", "Nature" } },
    { "a hidden hero", new List<string> { "Stealth", "Insight" } },
    { "a hesitant but loyal friend", new List<string> { "Insight", "Medicine" } },

    // Mentor-related (optional)
    { "an old assassin", new List<string> { "Stealth", "Deception" } },
    { "a secret order", new List<string> { "Arcana", "Religion" } },
    { "your uncle, a former adventurer", new List<string> { "Survival", "Athletics" } },
    { "temple priests", new List<string> { "Religion", "Medicine" } },
    { "a disguised dragon", new List<string> { "Arcana", "Intimidation" } },
    { "the warrior academy in the stone fortress", new List<string> { "Athletics", "History" } },
};
    public List<string> backgroundSkills = new List<string>();
    public List<RaceAgeData> raceAgeDataList = new List<RaceAgeData>
    {
        new RaceAgeData { race = "Human", minAge = 16, maxAge = 80, defaultAge = 25 },
        new RaceAgeData { race = "Elf", minAge = 50, maxAge = 1000, defaultAge = 120 },
        new RaceAgeData { race = "Goblin", minAge = 8, maxAge = 40, defaultAge = 20 },
        new RaceAgeData { race = "Orc", minAge = 12, maxAge = 60, defaultAge = 30 },
    };

    public float BodySizeSkinnyBlendValue = 0;
    public float BodySizeHeavyBlendValue = 0;
    public float MusclesBlendValue = 0;
    public float BodyTypeBlendValue = 0;

    public string selectedSpecies;
    public string selectedClass;
    public string firstName;
    public string lastName;
    public string age;
    public string backgroundSummary;
}

// För att lägga till fler delar till AllowedParts:
//void AddMoreHipsParts()
//{
//    AllowedParts[CharacterPartType.Hips].Add("Hips_Chainmail03");
//}