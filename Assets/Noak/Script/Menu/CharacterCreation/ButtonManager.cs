using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class ButtonManager : MonoBehaviour
    {
        [SerializeField] GameObject[] panels;
        [SerializeField] Button button;

        // Update is called once per frame
        void Update()
        {
            IsPanelActive();
        }

        private void IsPanelActive()
        {
            // Assume no panels are active by default
            bool anyPanelActive = false;

            foreach (GameObject panel in panels)
            {
                if (panel.activeSelf)
                {
                    anyPanelActive = true;
                    break;
                }
            }

            // Set the button interactability
            button.interactable = !anyPanelActive;
        }
    }
}
