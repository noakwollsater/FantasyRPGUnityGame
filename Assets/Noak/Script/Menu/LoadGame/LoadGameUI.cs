using TMPro;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class LoadGameUI : MonoBehaviour
    {
        public TextMeshProUGUI chapterNameText;
        public TextMeshProUGUI saveTypeText;
        public TextMeshProUGUI saveDateText;
        public TextMeshProUGUI characterNameText;
        public TextMeshProUGUI inGameTimeText;

        public void SetData(GameSaveData data)
        {
            chapterNameText.text = data.chapterName;
            saveTypeText.text = data.saveType.ToString();
            saveDateText.text = data.saveDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            characterNameText.text = data.characterFullName;

            int hours = Mathf.FloorToInt(data.inGameTimeMinutes / 60f);
            int minutes = Mathf.FloorToInt(data.inGameTimeMinutes % 60f);
            string timeFormatted = $"{hours:00}:{minutes:00}";

            //Beräkna månadsnamn på engelska.
            string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string monthName = monthNames[data.inGameMonth - 1];

            inGameTimeText.text = $"{data.inGameDay} {monthName} kl. {timeFormatted}";
        }
    }
}
