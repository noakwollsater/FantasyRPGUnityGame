using System.Collections.Generic;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class LoadCharacterData : MonoBehaviour
    {
        [SerializeField] private ZoomFunction _zoomFunction;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private HotbarStats _hotbarStats;

        private CharacterSaveData data;

        private readonly string saveKey = "MyCharacter";

        [Header("🧙 Character Info")]
        public string characterName;
        public int level;
        public float experience;
        public float experienceToNextLevel;

        [Header("📜 Role Info")]
        public string title;
        public string alignment;
        public string race;
        public string className;
        public string background;

        [Header("💪 Appearance")]
        public float muscle;
        public float skinny;
        public float fat;
        public float genderBlend;

        [Header("💰 Currency")]
        public int gold;
        public int silver;
        public int copper;
        public int bank;

        [Header("✅ Quests")]
        public List<string> completedQuests = new();

        [Header("📘 Background Skills")]
        public List<string> backgroundSkills = new();

        [Header("🎯 Final Attributes")]
        public AttributeSet finalAttributes;
        public ExtendedStats finalStats;
        public ExtendedStats currentStats;

        public bool isDead;

        void Start()
        {
            LoadCharacterSaveData();
            updateStats();
            SkillCheckers();
        }

        [ContextMenu("🔁 Load Character Data")]
        public void LoadCharacterSaveData()
        {
            string savedName = PlayerPrefs.GetString("SavedCharacterName", "Default");
            string folderName = $"PlayerSave_{savedName}"; 
            string fileName = $"{folderName}/CharacterSave_{savedName}.es3";

            var settings = new ES3Settings(fileName)
            {
                encryptionType = ES3.EncryptionType.AES,
                encryptionPassword = "K00a03j23s50a25" // ❗ Ensure this matches your save method
            };

            if (!ES3.FileExists(fileName) || !ES3.KeyExists(saveKey, settings))
            {
                Debug.LogWarning("⚠️ No character save found.");
                return;
            }

            data = ES3.Load<CharacterSaveData>(saveKey, settings);
            Debug.Log("✅ Character data loaded!");

            // General
            characterName = $"{data.firstName} {data.lastName}";
            race = data.race;
            className = data.className;
            background = data.background;

            // Stats
            level = data.level;
            experience = data.experience;
            experienceToNextLevel = data.experienceToNextLevel;

            // Role
            title = data.title;
            alignment = data.alignment;

            // Currency
            bank = data.bank;
            gold = data.gold;
            silver = data.silver;
            copper = data.copper;

            // Appearance
            muscle = data.muscle;
            skinny = data.skinny;
            fat = data.fat;
            genderBlend = data.genderBlend;

            // Quests and skills
            completedQuests = new List<string>(data.completedQuests);
            backgroundSkills = new List<string>(data.backgroundSkills);

            // Final attributes
            finalAttributes = data.finalAttributes;
            finalStats = data.finalStats;
            currentStats = data.currentStats;

            isDead = data.isDead;
        }

        private void updateStats()
        {
            _levelManager.UpdateAllUI(force: true);
            _hotbarStats.setAttributeManager();
        }

        private void SkillCheckers()
        {
            foreach (var skill in backgroundSkills)
            {
                if (skill == "Stealth")
                {
                    // Add your logic for Stealth skill here
                }
                else if (skill == "Sleight of Hand")
                {
                    // Add your logic for Sleight of Hand skill here
                }
                else if (skill == "Survival")
                {
                    _zoomFunction.EnableZoom();
                }
                else if (skill == "Investigation")
                {
                    _zoomFunction.EnableZoom();
                }
                else if (skill == "Arcana")
                {
                    // Add your logic for Arcana skill here
                }
                else if (skill == "History")
                {
                    // Add your logic for History skill here
                }
                else if (skill == "Persuasion")
                {
                    // Add your logic for Persuasion skill here
                }
                else if (skill == "Insight")
                {
                    // Add your logic for Insight skill here
                }
                else if (skill == "Medicine")
                {
                    // Add your logic for Medicine skill here
                }
                else if (skill == "Animal Handling")
                {
                    // Add your logic for Animal Handling skill here
                }
                else if (skill == "Deception")
                {
                    // Add your logic for Deception skill here
                }
                else if (skill == "Intimidation")
                {
                    // Add your logic for Intimidation skill here
                }
                else if (skill == "Nature")
                {
                    // Add your logic for Nature skill here
                }
                else if (skill == "Athletics")
                {
                    // Add your logic for Athletics skill here
                }
                else if (skill == "Perception")
                {
                    // Add your logic for Perception skill here
                }
                else if (skill == "Religion")
                {
                    //Add your logic for Religion skill here
                }
            }
        }
    }
}