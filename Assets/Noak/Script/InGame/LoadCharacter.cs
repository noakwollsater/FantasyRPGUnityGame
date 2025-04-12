using System.Collections.Generic;
using UnityEngine;
using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Enums;

namespace Unity.FantasyKingdom
{
    public class LoadCharacter : MonoBehaviour
    {
        private readonly string saveKey = "MyCharacter";

        private DatabaseManager _dbManager;
        private DictionaryLibrary _dictionaryLibrary;
        private CharacterSaveData data;
        private GameObject character;

        [SerializeField] private GameObject characterPrefab;


        private readonly string outputModelName = "Player";

        void Start()
        {
            LoadCharacterFromSave();
        }

        private void LoadCharacterFromSave()
        {
            string fileName = $"CharacterSave_{PlayerPrefs.GetString("SavedCharacterName", "Default")}.es3";
            var settings = new ES3Settings(fileName);

            if (!ES3.FileExists(fileName) || !ES3.KeyExists(saveKey, settings))
            {
                Debug.LogWarning("⚠️ Ingen sparad karaktär hittades.");
                return;
            }

            data = ES3.Load<CharacterSaveData>(saveKey, settings);
            Debug.Log("✅ Laddade sparad karaktär!");

            _dbManager = new DatabaseManager();

            character = GameObject.Find(outputModelName);
            if (character == null)
            {
                Debug.Log("📦 Instantiating character prefab...");
                character = Instantiate(characterPrefab, transform);
                character.name = outputModelName;
            }


            // Build library data
            _dictionaryLibrary = new DictionaryLibrary
            {
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

            ActivateSavedParts();
            SetBlendShapes();
        }

        private void ActivateSavedParts()
        {
            var skinnedMeshes = character.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            HashSet<string> activePartNames = new HashSet<string>(data.selectedParts.Values);

            foreach (var smr in skinnedMeshes)
            {
                bool shouldActivate = activePartNames.Contains(smr.name);
                smr.gameObject.SetActive(shouldActivate);
                Debug.Log($"{(shouldActivate ? "✅" : "❌")} {(shouldActivate ? "Activated" : "Deactivated")}: {smr.name}");
            }
        }

        private void SetBlendShapes()
        {
            foreach (var smr in character.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (!smr.gameObject.activeInHierarchy || smr.sharedMesh == null)
                    continue;

                for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
                {
                    string shapeName = smr.sharedMesh.GetBlendShapeName(i).ToLower();

                    if (shapeName.Contains("masculinefeminine"))
                    {
                        smr.SetBlendShapeWeight(i, data.genderBlend);
                    }
                    else if (shapeName.Contains("buff"))
                    {
                        smr.SetBlendShapeWeight(i, data.muscle);
                    }
                    else if (shapeName.Contains("skinny"))
                    {
                        smr.SetBlendShapeWeight(i, data.skinny);
                    }
                    else if (shapeName.Contains("heavy") || shapeName.Contains("fat"))
                    {
                        smr.SetBlendShapeWeight(i, data.fat);
                    }
                }
            }

            Debug.Log("✅ Blendshapes applied to all active parts!");
        }
    }
}
