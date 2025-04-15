using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Camera;

namespace Unity.FantasyKingdom
{
    public class LoadCharacter : MonoBehaviour
    {
        private readonly string saveKey = "MyCharacter";

        private DictionaryLibrary _dictionaryLibrary;
        [SerializeField] private CameraController _cameraController;
        private CharacterSaveData data;
        private GameObject character;

        [SerializeField] private GameObject characterPrefab;


        private readonly string outputModelName = "Player";

        void Start()
        {
            LoadCharacterFromSave();
            LoadGameDataFromSave();
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

            character = GameObject.Find(outputModelName);
            if (character == null)
            {
                Debug.Log("📦 Instantiating character prefab...");
                character = Instantiate(characterPrefab, transform);
                character.name = outputModelName;

                _cameraController.Character = character;

            }


            // Build library data
            _dictionaryLibrary = ScriptableObject.CreateInstance<DictionaryLibrary>();
            _dictionaryLibrary.selectedSpecies = data.race;
            _dictionaryLibrary.selectedClass = data.className;
            _dictionaryLibrary.firstName = data.firstName;
            _dictionaryLibrary.lastName = data.lastName;
            _dictionaryLibrary.age = data.age;
            _dictionaryLibrary.backgroundSummary = data.background;
            _dictionaryLibrary.backgroundSkills = data.backgroundSkills;
            _dictionaryLibrary.BodyTypeBlendValue = data.genderBlend;
            _dictionaryLibrary.BodySizeHeavyBlendValue = data.fat;
            _dictionaryLibrary.BodySizeSkinnyBlendValue = data.skinny;
            _dictionaryLibrary.MusclesBlendValue = data.muscle;


            ActivateSavedParts();
            SetBlendShapes();
        }
        private void ActivateSavedParts()
        {
            var skinnedMeshes = character.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            HashSet<string> activePartNames = new HashSet<string>();

            foreach (var kvp in data.selectedParts)
            {
                // ✅ Om värdet är "True" eller "False", använd key som mesh-namn
                if (bool.TryParse(kvp.Value, out bool isActive) && isActive)
                {
                    activePartNames.Add(kvp.Key);
                }
                // ✅ Annars antar vi att värdet är själva mesh-namnet
                else
                {
                    activePartNames.Add(kvp.Value);
                }
            }

            Debug.Log("🧩 Aktiva delar att matcha:");
            foreach (var name in activePartNames)
            {
                Debug.Log($"🧩 {name}");
            }

            foreach (var smr in skinnedMeshes)
            {
                string smrName = smr.name.Replace("(Clone)", "").Trim();
                bool shouldActivate = false;

                foreach (var partName in activePartNames)
                {
                    if (smrName.Contains(partName))
                    {
                        shouldActivate = true;
                        break;
                    }
                }

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

        private void LoadGameDataFromSave()
        {
            string lastSavedGameFile = PlayerPrefs.GetString("LastSavedGame", string.Empty);

            if (string.IsNullOrEmpty(lastSavedGameFile) || !ES3.FileExists(lastSavedGameFile))
            {
                Debug.LogWarning("⚠️ Inget sparat spel hittades.");
                return;
            }

            var settings = new ES3Settings(lastSavedGameFile);

            if (!ES3.KeyExists("GameSave", settings))
            {
                Debug.LogWarning("⚠️ Ingen GameSave-data hittades i filen.");
                return;
            }

            GameSaveData gameData = ES3.Load<GameSaveData>("GameSave", settings);

            Debug.Log($"✅ Game loaded! Kapitel: {gameData.chapterName}, Tid: {gameData.inGameTimeOfDay}");

            if (character != null)
            {
                character.transform.position = gameData.characterPosition;
                Debug.Log($"🚶‍♂️ Flyttade karaktär till sparad position: {gameData.characterPosition}");
            }
        }

    }
}
