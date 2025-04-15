using Synty.Interface.FantasyMenus.Samples;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

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
            inGameTimeText.text = data.inGameTimeOfDay;
        }
    }
}
