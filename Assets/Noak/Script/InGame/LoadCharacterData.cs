using System.Collections.Generic;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class LoadCharacterData : MonoBehaviour
    {
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

        void Start()
        {
            LoadCharacterSaveData();
        }

        [ContextMenu("🔁 Load Character Data")]
        private void LoadCharacterSaveData()
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

            // Selected parts (if needed for logic elsewhere)
            var selectedParts = data.selectedParts;
        }
    }
}