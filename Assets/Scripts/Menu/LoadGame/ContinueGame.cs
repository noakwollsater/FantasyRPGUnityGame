using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class ContinueGame : MonoBehaviour
    {
        [SerializeField] private Button continueGameBtn;

        void Start()
        {
            string basePath = Application.persistentDataPath;
            var dirs = Directory.GetDirectories(basePath, "PlayerSave_*");

            if (dirs.Length == 0)
            {
                Debug.Log("⚠️ Inga sparade karaktärer hittades.");
                continueGameBtn.interactable = false;
                return;
            }

            continueGameBtn.interactable = true;
            Debug.Log("✅ Minst en sparmapp hittades.");
            continueGameBtn.onClick.AddListener(OnClick);
        }



        private void OnClick()
        {
            SceneTransitionManager.Instance.LoadScene("Gameplay");
        }
    }
}
