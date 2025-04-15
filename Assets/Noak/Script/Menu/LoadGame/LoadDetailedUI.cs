using Synty.Interface.FantasyMenus.Samples;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
            inGameTimeText.text = data.inGameTimeOfDay;
        }
    }
}
