using Opsive.UltimateCharacterController.Character;
using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class ScriptAttacher : MonoBehaviour
    {
        [Header("Prefab Setup")]
        [SerializeField] private GameObject _characterPrefab; // Original prefab att kopiera
        [SerializeField] private Transform _spawnPoint;       // Plats att instansiera karaktären

        [Header("Animator Setup")]
        [SerializeField] private RuntimeAnimatorController _animatorController;

        [Header("Other References")]
        [SerializeField] private GameObject _playerObject;
        [SerializeField] private GameObject _inputObject;

        private GameObject _spawnedCharacter;
        private DatabaseManager _dbManager;
        private SidekickRuntime _sidekickRuntime;
        private DictionaryLibrary _dictionaryLibrary;

        private void Start()
        {
            if (_characterPrefab == null)
            {
                Debug.LogError("[ScriptAttacher] Ingen prefab tilldelad!");
                return;
            }

            SpawnAndSetupCharacter();
        }

        private void SpawnAndSetupCharacter()
        {
            // Rensa ev. tidigare instans
            if (_spawnedCharacter != null)
            {
                Destroy(_spawnedCharacter);
            }

            // Instansiera prefab
            Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : Vector3.zero;
            Quaternion spawnRot = _spawnPoint != null ? _spawnPoint.rotation : Quaternion.identity;

            _spawnedCharacter = Instantiate(_characterPrefab, spawnPos, spawnRot);
            _spawnedCharacter.name = "Sidekick Character";

            // Lägg till komponenter
            AddComponentIfMissing<Animator>(_spawnedCharacter);
            AddComponentIfMissing<AnimatorMonitor>(_spawnedCharacter);
            AddComponentIfMissing<CharacterIK>(_spawnedCharacter);
            AddComponentIfMissing<CharacterFootEffects>(_spawnedCharacter);

            // Koppla Animator Controller
            Animator animator = _spawnedCharacter.GetComponent<Animator>();
            if (_animatorController != null)
            {
                animator.runtimeAnimatorController = _animatorController;
                Debug.Log("[ScriptAttacher] Animator Controller tilldelad.");
            }
            else
            {
                Debug.LogWarning("[ScriptAttacher] Animator Controller saknas.");
            }

            // Initiera databasen om det behövs
            _dbManager = new DatabaseManager();
            _sidekickRuntime = new SidekickRuntime(_spawnedCharacter, null, _animatorController, _dbManager);

            Debug.Log("[ScriptAttacher] Karaktär instansierad och konfigurerad.");
        }

        // Hjälpmetod för att säkert lägga till komponenter
        private void AddComponentIfMissing<T>(GameObject obj) where T : Component
        {
            if (obj.GetComponent<T>() == null)
            {
                obj.AddComponent<T>();
                Debug.Log($"[ScriptAttacher] Lade till komponent: {typeof(T).Name}");
            }
        }
    }
}
