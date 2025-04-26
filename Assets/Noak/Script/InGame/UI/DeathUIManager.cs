using UnityEngine;
using Opsive.Shared.Events; // Viktigt för att lyssna på EventHandler
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Traits;
using Unity.FantasyKingdom;
using System.Collections;

public class DeathUIManager : MonoBehaviour
{
    [SerializeField] public GameObject deathPanel;
    [SerializeField] public GameObject ingameUI;
    [SerializeField] private SaveGameManager saveGameManager;
    [SerializeField] private LoadCharacterData loadCharacterData;
    [SerializeField] private HotbarStats hotbarStats;

    private GameObject m_Character;
    public CharacterRespawner m_Respawner;

    public void Init()
    {
        m_Character = GameObject.FindGameObjectWithTag("Player");
        m_Respawner = m_Character.GetComponent<CharacterRespawner>();

        if (m_Respawner == null)
        {
            Debug.LogError("Respawner komponent saknas på karaktären!");
        }

        deathPanel.SetActive(false);

        // Registrera att lyssna på OnDeath
        EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(m_Character, "OnDeath", OnDeath);
    }

    private void OnDestroy()
    {
        // Avregistrera eventet när detta objekt förstörs
        EventHandler.UnregisterEvent<Vector3, Vector3, GameObject>(m_Character, "OnDeath", OnDeath);
    }

    private void OnDeath(Vector3 position, Vector3 force, GameObject attacker)
    {
        StartCoroutine(ShowDeathPanelAfterDelay());
    }

    public IEnumerator ShowDeathPanelAfterDelay()
    {
        yield return new WaitForSeconds(1f); // Vänta 1 sekund

        ingameUI.SetActive(false);
        deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void RespawnButtonPressed()
    {
        if (m_Respawner != null)
        {
            loadCharacterData.isDead = false;
            loadCharacterData.currentStats.HP = 50;
            saveGameManager.SaveCharacterData();
            m_Respawner.Respawn();
            deathPanel.SetActive(false);
            ingameUI.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            loadCharacterData.LoadCharacterSaveData();
            loadCharacterData.updateStats();
        }
    }

    public void QuitButtonPressed()
    {
        loadCharacterData.isDead = true;
        saveGameManager.SaveCharacterData();
        saveGameManager.SaveGameData("Start of the Journey", "Heimdal", SaveType.Manual, "00:00", m_Character.transform);
        PlayerPrefs.SetInt("ReturnToMainMenu", 1);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
