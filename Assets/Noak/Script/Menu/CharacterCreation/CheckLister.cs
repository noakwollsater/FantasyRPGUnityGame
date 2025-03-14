using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class CheckLister : MonoBehaviour
    {
        [SerializeField] private GameObject[] checkListIcons;

        [SerializeField] private Button firstNextBtn;
        [SerializeField] private Button secondNextBtn;
        //[SerializeField] private Button thirdNextBtn;
        //[SerializeField] private Button fourthNextBtn;
        //[SerializeField] private Button fifthNextBtn;

        [SerializeField] private Button firstBackBtn;
        [SerializeField] private Button secondBackBtn;
        //[SerializeField] private Button thirdBackBtn;
        //[SerializeField] private Button fourthBackBtn;
        //[SerializeField] private Button fifthBackBtn;

        private int currentStage = 1;
        void Start()
        {
            firstNextBtn.onClick.AddListener(NextStage);
            secondNextBtn.onClick.AddListener(NextStage);
            //thirdNextBtn.onClick.AddListener(NextStage);
            //fourthNextBtn.onClick.AddListener(NextStage);
            //fifthNextBtn.onClick.AddListener(NextStage);

            firstBackBtn.onClick.AddListener(PreviousStage);
            secondBackBtn.onClick.AddListener(PreviousStage);
            //thirdBackBtn.onClick.AddListener(PreviousStage);
            //fourthBackBtn.onClick.AddListener(PreviousStage);
            //fifthBackBtn.onClick.AddListener(PreviousStage);
        }

        private void NextStage()
        {
            if (currentStage < checkListIcons.Length)
            {
                checkListIcons[currentStage - 1].SetActive(true);
                currentStage++;
            }
        }

        private void PreviousStage()
        {
            if (currentStage > 1)
            {
                checkListIcons[currentStage - 2].SetActive(false);
                currentStage--;
            }
        }
    }
}
