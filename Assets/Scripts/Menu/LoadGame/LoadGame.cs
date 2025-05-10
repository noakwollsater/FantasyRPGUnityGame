using Synty.Interface.FantasyMenus.Samples;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class LoadGame : MonoBehaviour
    {
        public SamplePopup samplePopup;
        public GameObject loadgame;
        public GameObject confirm;

        public GameSaveData saveData;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            loadgame = transform.parent.parent.parent.gameObject;
            confirm = loadgame.transform.parent.Find("Load_Confirmation")?.gameObject;
            if (confirm != null)
            {
                samplePopup = confirm.GetComponent<SamplePopup>();
            }
            else
            {
                Debug.LogError("confirm is null");
            }
        }
        public void OnClickselectGameBtn()
        {
            if (samplePopup == null)
            {
                Debug.LogError("samplePopup is null!");
                return;
            }

            if (saveData == null)
            {
                Debug.LogError("saveData is null!");
                return;
            }

            // Sätt PlayerPrefs
            PlayerPrefs.SetString("SavedCharacterName", saveData.characterFullName);
            PlayerPrefs.SetString("LastSavedGame", $"GameSave_{saveData.characterFullName}.es3");

            Debug.Log($"✅ Sparade PlayerPrefs: {saveData.characterFullName}");

            // Visa popup
            samplePopup.ShowMe();
            Debug.Log("✅ Popup is klicked");
        }
    }
}
