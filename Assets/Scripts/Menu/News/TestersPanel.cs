using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class TestersPanel : MonoBehaviour
    {
        [SerializeField] private GameObject TesterPanel;
        [SerializeField] private Button Adventure;
        [SerializeField] private Button Anm�lProblem;
        [SerializeField] private Button News;
        [SerializeField] private Button Multiplayer;
        [SerializeField] private Button CloseButton;

        string url = "https://forms.gle/2BkJtEjoUQNXmvAW6";
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Adventure.interactable = false;
            Anm�lProblem.interactable = false;
            News.interactable = false;
            Multiplayer.interactable = false;

            CloseButton.onClick.AddListener(() => CloseMeny());
            Anm�lProblem.onClick.AddListener(() => SendSendToForm());
        }

        private void CloseMeny()
        {
            Adventure.interactable = true;
            Anm�lProblem.interactable = true;
            News.interactable = true;
            Multiplayer.interactable = true;
            TesterPanel.SetActive(false);
        }

        private void SendSendToForm()
        {
            Application.OpenURL(url);
        }
    }
}
