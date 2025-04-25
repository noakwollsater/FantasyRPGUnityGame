using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Traits;
using Unity.FantasyKingdom;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [SerializeField] LoadCharacterData _characterLoadData;
    [SerializeField] CharacterHealth _characterHealth;

    [SerializeField] GameObject _deathScreen;

    private GameObject _characterObject;

    private void Start()
    {
        _characterObject = GameObject.FindGameObjectWithTag("Player");
        if (_characterObject == null)
        {
            Debug.LogError("Player object not found in the scene.");
            return;
        }
        
    }

}