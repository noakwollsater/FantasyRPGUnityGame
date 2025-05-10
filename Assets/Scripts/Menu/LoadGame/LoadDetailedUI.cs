using TMPro;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class LoadDetailedUI : MonoBehaviour
    {
        public TextMeshProUGUI chapterNameText;
        public TextMeshProUGUI areaNameText;
        public TextMeshProUGUI saveTypeText;
        public TextMeshProUGUI saveDateText;
        public TextMeshProUGUI characterNameText;
        public TextMeshProUGUI inGameTimeText;

        public void SetData(GameSaveData data)
        {
            chapterNameText.text = data.chapterName;
            areaNameText.text = data.areaName;
            saveTypeText.text = data.saveType.ToString();
            saveDateText.text = data.saveDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            characterNameText.text = data.characterFullName;
            int hours = Mathf.FloorToInt(data.inGameTimeMinutes / 60f);
            int minutes = Mathf.FloorToInt(data.inGameTimeMinutes % 60f);
            string timeFormatted = $"{hours:00}:{minutes:00}";

            //Beräkna månadsnamn på engelska.
            string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string monthName = "Unknown";
            if (data.inGameMonth >= 1 && data.inGameMonth <= 12)
            {
                monthName = monthNames[data.inGameMonth - 1];
            }
            else
            {
                Debug.LogWarning($"Invalid inGameMonth: {data.inGameMonth}. Must be between 1 and 12.");
            }


            inGameTimeText.text = $"{data.inGameDay} {monthName} kl. {timeFormatted}";
        }
    }
}
