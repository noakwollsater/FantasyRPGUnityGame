using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Camera;
using Mightland.Scripts.SK;
using FIMSpace.FTail;

namespace Unity.FantasyKingdom
{
    public enum TailType
    {
        Default,
        Torso,
        Hips
    }

    public class LoadCharacter : MonoBehaviour
    {
        [SerializeField] SidekickConfigurator _sidekickConfigurator;

        private readonly string saveKey = "MyCharacter";
        private const string encryptionPassword = "K00a03j23s50a25";

        private DictionaryLibrary _dictionaryLibrary;
        [SerializeField] private CameraController _cameraController;
        private CharacterSaveData data;
        private GameObject character;

        [SerializeField] private GameObject characterPrefab;
        private Transform root;


        private readonly string outputModelName = "Player";

        void Start()
        {
            LoadCharacterFromSave();
            LoadGameDataFromSave();
        }

        private void LoadCharacterFromSave()
        {
            string characterName = PlayerPrefs.GetString("SavedCharacterName", "Default");
            string folderName = $"PlayerSave_{characterName}";
            string fileName = $"{folderName}/CharacterSave_{characterName}.es3";

            var settings = new ES3Settings(fileName)
            {
                encryptionType = ES3.EncryptionType.AES,
                encryptionPassword = encryptionPassword
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

                Transform player = character.transform.Find("Player");
                root = player.transform.Find("root");
                if (root != null)
                {
                    AttachTailAnimators(root);
                }
                else
                {
                    Debug.LogError("Root transform not found in character model.");
                }

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
        public void AttachTailAnimators(Transform root)
        {
            if (root == null)
            {
                Debug.LogWarning("AttachTailAnimators: root is null");
                return;
            }

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child == null) continue;

                string name = child.name?.ToLower();
                if (!string.IsNullOrEmpty(name) && name.Contains("dyn"))
                {
                    if (!child.TryGetComponent<TailAnimator2>(out _))
                    {
                        TailAnimator2 tailAnimator = child.gameObject.AddComponent<TailAnimator2>();

                        // Different configs based on name
                        if (name.Contains("tors"))
                        {
                            ConfigureTailAnimator(tailAnimator, TailType.Torso);
                        }
                        else if (name.Contains("hips"))
                        {
                            ConfigureTailAnimator(tailAnimator, TailType.Hips);
                        }
                        else
                        {
                            ConfigureTailAnimator(tailAnimator, TailType.Default);
                        }

                        Debug.Log($"TailAnimator2 added to {child.name}");
                    }
                }
            }
        }

        private void ConfigureTailAnimator(TailAnimator2 tail, TailType type)
        {
            if (tail == null) return;

            tail.UseWaving = false;
            tail.IKAutoWeights = true;
            tail.IKAutoAngleLimits = true;

            switch (type)
            {
                case TailType.Torso:
                    tail.MotionInfluence = 0f;
                    tail.ReactionSpeed = 0.7f;
                    tail.MaxStretching = 0.15f;
                    break;

                case TailType.Hips:
                    tail.MotionInfluence = 0.15f;
                    tail.ReactionSpeed = 0.7f;
                    tail.MaxStretching = 0.15f;
                    break;

                default:
                    tail.MotionInfluence = 0.3f;
                    tail.ReactionSpeed = 0.7f;
                    tail.MaxStretching = 0.15f;
                    break;
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
            string savedName = PlayerPrefs.GetString("SavedCharacterName", "Default");
            string folderName = $"PlayerSave_{savedName}";
            string fileName = $"{folderName}/GameSave_{savedName}.es3";

            if (!ES3.FileExists(fileName))
            {
                Debug.LogWarning("⚠️ Inget sparat spel hittades.");
                return;
            }

            var settings = new ES3Settings(fileName)
            {
                encryptionType = ES3.EncryptionType.AES,
                encryptionPassword = encryptionPassword // Use the const if you've centralized it
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
