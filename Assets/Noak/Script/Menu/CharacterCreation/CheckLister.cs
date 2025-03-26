using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class CheckLister : MonoBehaviour
    {
        [SerializeField] private GameObject[] checkListIcons;  // Checkmarks or stage indicators
        [SerializeField] private GameObject[] stagePanels;     // Panels/pages for each stage

        [SerializeField] private GameObject raceStatsPanel;
        [SerializeField] private GameObject classStatsPanel;

        [SerializeField] private Button nextBtn;
        [SerializeField] private Button backBtn;

        private int currentStage = 0; // Start at 0 for array indexing

        void Start()
        {
            nextBtn.onClick.AddListener(NextStage);
            backBtn.onClick.AddListener(PreviousStage);

            UpdateStage();
        }

        private void NextStage()
        {
            if (currentStage < stagePanels.Length - 1)
            {
                currentStage++;
                UpdateStage();
            }
        }

        private void PreviousStage()
        {
            if (currentStage > 0)
            {
                currentStage--;
                UpdateStage();
            }
        }

        private void UpdateStage()
        {
            // Activate only the current stage panel
            for (int i = 0; i < stagePanels.Length; i++)
            {
                stagePanels[i].SetActive(i == currentStage);
            }

            // Update checklist icons
            for (int i = 0; i < checkListIcons.Length; i++)
            {
                checkListIcons[i].SetActive(i <= currentStage - 1);
            }

            // Navigation button interactivity
            backBtn.interactable = currentStage > 0;
            nextBtn.interactable = currentStage < stagePanels.Length - 1;

            // === 🧠 Stat Panel Logic ===
            // Show race panel on step 0 (race) and last step (background)
            if (currentStage == 0 || currentStage == stagePanels.Length - 1)
            {
                raceStatsPanel.SetActive(true);
                classStatsPanel.SetActive(false);
            }
            // Show class panel on step 1 (class)
            else if (currentStage == 2)
            {
                raceStatsPanel.SetActive(false);
                classStatsPanel.SetActive(true);
            }
            else
            {
                raceStatsPanel.SetActive(true);
                classStatsPanel.SetActive(false);
            }
        }
    }
}
