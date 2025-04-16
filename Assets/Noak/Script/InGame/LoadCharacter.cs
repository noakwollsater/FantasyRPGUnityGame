using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Camera;
using Mightland.Scripts.SK;

namespace Unity.FantasyKingdom
{
    public class LoadCharacter : MonoBehaviour
    {
        [SerializeField] SidekickConfigurator _sidekickConfigurator;

        private readonly string saveKey = "MyCharacter";
        private const string encryptionPassword = "MySuperSecretPassword123!";

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
            var settings = new ES3Settings(fileName)
            {
                encryptionType = ES3.EncryptionType.AES,
                encryptionPassword = "MySuperSecretPassword123!"
            };

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

                _sidekickConfigurator = character.GetComponentInChildren<SidekickConfigurator>();
                if (_sidekickConfigurator == null)
                {
                    Debug.LogError("❌ SidekickConfigurator saknas på karaktären!");
                    return;
                }
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

            // ⬇️ Tilldela blendshape-data till SidekickConfigurator
            _sidekickConfigurator.bodyTypeBlendValue = data.genderBlend;
            _sidekickConfigurator.bodySizeValue = data.fat - data.skinny;
            _sidekickConfigurator.musclesBlendValue = data.muscle;

            ApplySavedPartsToSidekickConfigurator();
            SetBlendShapes();
        }
        private void ApplySavedPartsToSidekickConfigurator()
        {
            if (_sidekickConfigurator == null || data.selectedParts == null)
            {
                Debug.LogWarning("⚠️ SidekickConfigurator eller sparade delar saknas.");
                return;
            }

            for (int i = 0; i < _sidekickConfigurator.partGroups.Count; i++)
            {
                string partGroup = _sidekickConfigurator.partGroups[i];
                List<SidekickMesh> groupParts = _sidekickConfigurator.meshPartsList[i].items;

                for (int j = 0; j < groupParts.Count; j++)
                {
                    if (groupParts[j] == null) continue;

                    string meshName = groupParts[j].partName;
                    bool match = false;

                    foreach (var kvp in data.selectedParts)
                    {
                        // Stöd både boolean och direkt namn
                        if (bool.TryParse(kvp.Value, out bool isActive))
                        {
                            if (isActive && meshName.Contains(kvp.Key))
                                match = true;
                        }
                        else if (meshName.Contains(kvp.Value))
                        {
                            match = true;
                        }

                        if (match)
                        {
                            // Avaktivera tidigare aktiv
                            if (_sidekickConfigurator.meshPartsList[i].items[_sidekickConfigurator.meshPartsActive[i]]?.meshTransform != null)
                            {
                                _sidekickConfigurator.meshPartsList[i].items[_sidekickConfigurator.meshPartsActive[i]].meshTransform.gameObject.SetActive(false);
                            }

                            _sidekickConfigurator.meshPartsActive[i] = j;
                            groupParts[j].meshTransform.gameObject.SetActive(true);
                            break;
                        }
                    }
                }
            }

            _sidekickConfigurator.ApplyBlendShapes();
            Debug.Log("✅ Sparade mesh-delar tillämpade!");
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

            var settings = new ES3Settings(lastSavedGameFile)
            {
                encryptionType = ES3.EncryptionType.AES,
                encryptionPassword = "MySuperSecretPassword123!"
            };

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
