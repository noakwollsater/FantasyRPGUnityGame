using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class SaveGameManager : MonoBehaviour
    {
        [SerializeField] private LoadCharacterData characterData;
        [SerializeField] private GetActiveParts getActiveParts;
        [SerializeField] private GetActiveColors getActiveColors;

        private readonly string saveKey = "MyCharacter";
        private const string encryptionPassword = "K00a03j23s50a25";

        [ContextMenu("💾 Save Character Data")]
        public void SaveCharacterData()
        {
            if (characterData == null)
            {
                Debug.LogError("❌ Character data reference is missing!");
                return;
            }

            getActiveParts.GetActivePartsFromCharacter();
            getActiveColors.GetColorsFromRuntime();

            CharacterSaveData data = new CharacterSaveData
            {
                firstName = characterData.characterName.Split(' ')[0],
                lastName = characterData.characterName.Split(' ').Length > 1 ? characterData.characterName.Split(' ')[1] : "",
                race = characterData.race,
                className = characterData.className,
                background = characterData.background,

                level = characterData.level,
                experience = characterData.experience,
                experienceToNextLevel = characterData.experienceToNextLevel,

                title = characterData.title,
                alignment = characterData.alignment,

                bank = characterData.bank,
                gold = characterData.gold,
                silver = characterData.silver,
                copper = characterData.copper,

                muscle = characterData.muscle,
                skinny = characterData.skinny,
                fat = characterData.fat,
                genderBlend = characterData.genderBlend,

                completedQuests = new List<string>(characterData.completedQuests),
                backgroundSkills = new List<string>(characterData.backgroundSkills),

                finalAttributes = characterData.finalAttributes,
                finalStats = characterData.finalStats,
                currentStats = characterData.currentStats,

                selectedParts = getActiveParts.selectedParts,

                selectedColors = new Dictionary<string, string>(getActiveColors.selectedColors),

                isDead = characterData.isDead
        };

            string characterName = characterData.characterName;
            string folderName = $"PlayerSave_{characterName}";
            string fileName = $"{folderName}/CharacterSave_{characterName}.es3";

            var settings = new ES3Settings(fileName)
            {
                encryptionType = ES3.EncryptionType.AES,
                encryptionPassword = encryptionPassword
            };

            PlayerPrefs.SetString("SavedCharacterName", characterName);
            PlayerPrefs.Save();

            ES3.Save(saveKey, data, settings);
            Debug.Log($"✅ Character data saved to: {fileName}");
        }

        public void SaveGameData(string chapterName, string areaName, SaveType saveType, string inGameTimeOfDay, Transform characterTransform)
        {

            if (characterData == null)
            {
                Debug.LogError("❌ characterData is null! Cannot save.");
                return;
            }

            var timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                Debug.LogError("❌ TimeManager not found! Cannot save time.");
                return;
            }

            GameSaveData saveData = new GameSaveData
            {
                chapterName = chapterName,
                areaName = areaName,
                saveType = saveType,
                saveDateTime = DateTime.Now,
                inGameYear = timeManager.year,
                inGameMonth = timeManager.month,
                inGameDay = timeManager.dayCount,
                inGameTimeMinutes = timeManager.currentTime,
                characterFullName = characterData.characterName,
                characterPosition = characterTransform.position,
            };

            string characterName = characterData.characterName;
            string folderName = $"PlayerSave_{characterName}";
            string fileName = $"{folderName}/GameSave_{characterName}.es3";

            var settings = new ES3Settings(fileName)
            {
                encryptionType = ES3.EncryptionType.AES,
                encryptionPassword = encryptionPassword
            };

            ES3.Save("GameSave", saveData, settings);
            PlayerPrefs.SetString("LastSavedGame", fileName);
            PlayerPrefs.Save();

            Debug.Log($"✅ Game data saved to: {fileName}");
        }
    }
}
