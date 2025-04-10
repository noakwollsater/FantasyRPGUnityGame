using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Enums;

namespace Unity.FantasyKingdom
{
    public class LoadCharacter : MonoBehaviour
    {
        private readonly string saveKey = "MyCharacter";

        private SidekickRuntime _sidekickRuntime;
        private DictionaryLibrary _dictionaryLibrary;
        private DatabaseManager _dbManager;

        private readonly string outputModelName = "Player";

        private CharacterSaveData data;
        private GameObject character;               // Your Sidekick-generated runtime player

        void Start()
        {
            LoadCharacterFromSave();
        }


        private void LoadCharacterFromSave()
        {

            string fileName = $"CharacterSave_{PlayerPrefs.GetString("SavedCharacterName", "Default")}.es3";
            var settings = new ES3Settings(fileName);

            var keys = ES3.GetKeys(settings);
            Debug.Log("📦 Keys found in save file: " + string.Join(", ", keys));


            var allKeys = ES3.GetKeys(fileName);
            Debug.Log("📦 Keys in file: " + string.Join(", ", allKeys));


            Debug.Log($"📁 Trying to load file: {fileName}");
            Debug.Log($"📂 Full path: {System.IO.Path.Combine(Application.persistentDataPath, fileName)}");

            if (!ES3.FileExists(fileName) || !ES3.KeyExists(saveKey, settings))
            {
                Debug.LogWarning("Ingen sparad karaktär hittades.");
                return;
            }


            data = ES3.Load<CharacterSaveData>(saveKey, settings);
            Debug.Log("✅ Laddade sparad karaktär!");

            // Initiera databasen
            _dbManager = new DatabaseManager();

            GameObject baseModel = Resources.Load<GameObject>("Meshes/SK_BaseModel");
            Material baseMaterial = Resources.Load<Material>("Materials/M_BaseMaterial");

            _sidekickRuntime = new SidekickRuntime(baseModel, baseMaterial, null, _dbManager);

            _dictionaryLibrary = new DictionaryLibrary
            {
                _partLibrary = _sidekickRuntime.PartLibrary,
                _availablePartDictionary = new(),
                _partIndexDictionary = new(),
                selectedSpecies = data.race,
                selectedClass = data.className,
                firstName = data.firstName,
                lastName = data.lastName,
                age = data.age,
                backgroundSummary = data.background,
                backgroundSkills = data.backgroundSkills,
                BodyTypeBlendValue = data.genderBlend,
                BodySizeHeavyBlendValue = data.fat,
                BodySizeSkinnyBlendValue = data.skinny,
                MusclesBlendValue = data.muscle
            };

            // Bygg karaktärsdelar
            List<SkinnedMeshRenderer> partsToUse = new();

            foreach (var part in data.selectedParts)
            {
                if (!System.Enum.TryParse(part.Key, out CharacterPartType type))
                    continue;

                if (_dictionaryLibrary._partLibrary.TryGetValue(type, out var parts) && parts.ContainsKey(part.Value))
                {
                    string resourcePath = parts[part.Value];
                    GameObject partGO = Resources.Load<GameObject>(resourcePath);
                    if (partGO)
                    {
                        partsToUse.Add(partGO.GetComponentInChildren<SkinnedMeshRenderer>());
                    }

                    // Spara till dictionary så vi kan göra förändringar senare
                    _dictionaryLibrary._availablePartDictionary[type] = new Dictionary<string, string> { { part.Value, resourcePath } };
                    _dictionaryLibrary._partIndexDictionary[type] = 0;
                }
            }

            character = GameObject.Find(outputModelName);
            if (character != null) Destroy(character);

            character = _sidekickRuntime.CreateCharacter(outputModelName, partsToUse, false, true);
            SetSize(); // ⬅️ From CharacterCreation.cs
            ForceBlendshapeUpdate(character);
        }
        private void SetSize()
        {
            _sidekickRuntime.BodyTypeBlendValue = _dictionaryLibrary.BodyTypeBlendValue;
            _sidekickRuntime.BodySizeHeavyBlendValue = _dictionaryLibrary.BodySizeHeavyBlendValue;
            _sidekickRuntime.BodySizeSkinnyBlendValue = _dictionaryLibrary.BodySizeSkinnyBlendValue;
            _sidekickRuntime.MusclesBlendValue = _dictionaryLibrary.MusclesBlendValue;
        }

        private void ForceBlendshapeUpdate(GameObject character)
        {
            foreach (var smr in character.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (smr.sharedMesh == null) continue;

                for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
                {
                    string shapeName = smr.sharedMesh.GetBlendShapeName(i).ToLower();

                    if (shapeName.Contains("masculinefeminine"))
                    {
                        smr.SetBlendShapeWeight(i, data.genderBlend);
                        Debug.Log($"🟣 Set GenderBlend on {smr.name} → {shapeName}: {data.genderBlend}");
                    }
                    else if (shapeName.Contains("buff"))
                    {
                        smr.SetBlendShapeWeight(i, data.muscle);
                        Debug.Log($"🔴 Set Muscle on {smr.name} → {shapeName}: {data.muscle}");
                    }
                    else if (shapeName.Contains("skinny"))
                    {
                        smr.SetBlendShapeWeight(i, data.skinny);
                        Debug.Log($"🟢 Set Skinny on {smr.name} → {shapeName}: {data.skinny}");
                    }
                    else if (shapeName.Contains("heavy") || shapeName.Contains("fat"))
                    {
                        smr.SetBlendShapeWeight(i, data.fat);
                        Debug.Log($"🟡 Set Fat on {smr.name} → {shapeName}: {data.fat}");
                    }
                }
            }

            Debug.Log("✅ All blendshapes applied!");
        }
    }
}
