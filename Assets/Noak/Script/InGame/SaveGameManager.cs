using System;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class SaveGameManager : MonoBehaviour
    {
        [SerializeField] private LoadCharacterData characterData; // Dra in via Inspector eller hitta via kod
        [SerializeField] private GetActiveParts getActiveParts;

        private readonly string saveKey = "MyCharacter";

        [ContextMenu("💾 Save Character Data")]
        public void SaveCharacterData()
        {
            if (characterData == null)
            {
                Debug.LogError("❌ Character data reference is missing!");
                return;
            }

            getActiveParts.GetActivePartsFromCharacter();

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

                completedQuests = new System.Collections.Generic.List<string>(characterData.completedQuests),
                backgroundSkills = new System.Collections.Generic.List<string>(characterData.backgroundSkills),

                finalAttributes = characterData.finalAttributes,

                selectedParts = getActiveParts.selectedParts
            };

            string savedName = PlayerPrefs.GetString("SavedCharacterName", "Default");
            string fileName = $"CharacterSave_{savedName}.es3";
            var settings = new ES3Settings(fileName);

            ES3.Save(saveKey, data, settings);
            Debug.Log("✅ Character data saved!");
        }

        public void SaveGame(string chapterName, SaveType saveType, string inGameTimeOfDay, Transform characterTransform)
        {
            if (characterData == null)
            {
                Debug.LogError("❌ characterData is null! Cannot save.");
                return;
            }

            GameSaveData saveData = new GameSaveData
            {
                chapterName = chapterName,
                saveType = saveType,
                saveDateTime = DateTime.Now,
                inGameTimeOfDay = inGameTimeOfDay,
                characterFullName = $"{characterData.name}",
                characterPosition = characterTransform.position,
            };


            string fileName = $"GameSave_{characterData.name}.es3";
            var settings = new ES3Settings(fileName);

            ES3.Save("GameSave", saveData, settings);
            PlayerPrefs.SetString("LastSavedGame", fileName);
            PlayerPrefs.Save();

            Debug.Log("✅ Game saved with character!");
        }
    }
}
