using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class ContinueGame : MonoBehaviour
    {
        [SerializeField] private Button continueGameBtn;

        void Start()
        {
            string lastSavedGameFile = PlayerPrefs.GetString("LastSavedGame", string.Empty);
            string charactername = PlayerPrefs.GetString("SavedCharacterName", "Default");

            if (string.IsNullOrEmpty(charactername))
            {
                continueGameBtn.interactable = false;
                Debug.Log("⚠️ Ingen sparad fil hittades.");
            }
            else
            {
                continueGameBtn.interactable = true;
                Debug.Log($"✅ Sparad fil hittades: {lastSavedGameFile}");
                Debug.Log($"✅ Sparad karaktär hittades: {charactername}");
                continueGameBtn.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            SceneTransitionManager.Instance.LoadScene("Gameplay");
        }
    }
}
