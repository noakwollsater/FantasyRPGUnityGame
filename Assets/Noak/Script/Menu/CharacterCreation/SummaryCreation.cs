using Synty.SidekickCharacters.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static DictionaryLibrary;

namespace Unity.FantasyKingdom
{
    public class SummaryCreation : CharacterCreation
    {
        [SerializeField] private GameObject ThankYouForTestingMyGamePanel;

        [Header("Summary UI")]
        [SerializeField] private TMP_Text raceNameText;
        [SerializeField] private TMP_Text classNameText;
        [SerializeField] private TMP_InputField firstNameInput;
        [SerializeField] private TMP_InputField lastNameInput;

        [Header("Age UI")]
        [SerializeField] private Slider ageSlider;
        [SerializeField] private TMP_Text ageValueText; // Text that shows the current age
        private bool suppressSliderCallback = false; // Prevent OnValueChanged when programmatically changing slider

        [Header("Background Skills UI")]
        [SerializeField] private TMP_Text[] backgroundSkillsTexts;

        void Start()
        {
            UpdateSummary();
        }

        public void UpdateSummary()
        {
            if (_dictionaryLibrary == null) return;

            raceNameText.text = _dictionaryLibrary.selectedSpecies;
            classNameText.text = _dictionaryLibrary.selectedClass;
            // Fill input fields if names already exist
            if (!string.IsNullOrEmpty(_dictionaryLibrary.firstName))
                firstNameInput.text = _dictionaryLibrary.firstName;

            if (!string.IsNullOrEmpty(_dictionaryLibrary.lastName))
                lastNameInput.text = _dictionaryLibrary.lastName;


            if (TryGetRaceAgeData(_dictionaryLibrary.selectedSpecies, out var ageData))
            {
                suppressSliderCallback = true;

                ageSlider.minValue = ageData.minAge;
                ageSlider.maxValue = ageData.maxAge;

                int ageToSet;

                // If age is empty OR invalid, fall back to default
                if (string.IsNullOrEmpty(_dictionaryLibrary.age) || !int.TryParse(_dictionaryLibrary.age, out var parsedAge))
                {
                    ageToSet = ageData.defaultAge;
                    _dictionaryLibrary.age = ageToSet.ToString();
                }
                else
                {
                    ageToSet = Mathf.Clamp(parsedAge, ageData.minAge, ageData.maxAge);
                }

                // Safely assign slider value
                ageSlider.value = ageToSet;
                ageValueText.text = ageToSet.ToString();

                suppressSliderCallback = false;
            }

            var skills = _dictionaryLibrary.backgroundSkills;

            for (int i = 0; i < backgroundSkillsTexts.Length; i++)
            {
                Transform row = backgroundSkillsTexts[i].transform.parent.parent.parent;
                if (i < skills.Count)
                {
                    backgroundSkillsTexts[i].text = skills[i];
                    row.gameObject.SetActive(true);
                }
                else
                {
                    row.gameObject.SetActive(false);
                }
            }
        }

        public void OnFirstNameChanged(string value)
        {
            _dictionaryLibrary.firstName = Capitalize(value);
        }

        public void OnLastNameChanged(string value)
        {
            _dictionaryLibrary.lastName = Capitalize(value);
        }

        private string Capitalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        public void OnAgeSliderChanged(float value)
        {
            if (suppressSliderCallback) return;

            int roundedAge = Mathf.RoundToInt(value);
            ageValueText.text = roundedAge.ToString();
            _dictionaryLibrary.age = roundedAge.ToString();
        }


        private bool TryGetRaceAgeData(string race, out RaceAgeData data)
        {
            if (_dictionaryLibrary.raceAgeDataList != null)
            {
                foreach (var item in _dictionaryLibrary.raceAgeDataList)
                {
                    if (item.race == race)
                    {
                        data = item;
                        return true;
                    }
                }
            }

            data = null;
            return false;
        }

        public void SaveCharacter(string saveKey = "MyCharacter")
        {
            string fileName = $"CharacterSave_{_dictionaryLibrary.firstName + " " + _dictionaryLibrary.lastName}.es3";

            // Save playerprefs reference to use during loading
            PlayerPrefs.SetString("SavedCharacterName", _dictionaryLibrary.firstName + " " + _dictionaryLibrary.lastName);
            PlayerPrefs.Save();

            // 🔐 Set up encryption
            var settings = new ES3Settings(fileName)
            {
                //encryptionType = ES3.EncryptionType.AES,
                //encryptionPassword = "MySuperSecretPassword123!" // You should store this more securely in a real project
            };

            CharacterSaveData data = new CharacterSaveData
            {
                firstName = _dictionaryLibrary.firstName,
                lastName = _dictionaryLibrary.lastName,
                age = _dictionaryLibrary.age,
                race = _dictionaryLibrary.selectedSpecies,
                className = _dictionaryLibrary.selectedClass,

                muscle = _dictionaryLibrary.MusclesBlendValue,
                skinny = _dictionaryLibrary.BodySizeSkinnyBlendValue,
                fat = _dictionaryLibrary.BodySizeHeavyBlendValue,
                genderBlend = _dictionaryLibrary.BodyTypeBlendValue,

                background = _dictionaryLibrary.backgroundSummary,

                backgroundSkills = new List<string>(_dictionaryLibrary.backgroundSkills),
                finalAttributes = RaceSelectionUI.GetFinalAttributes(),

                selectedParts = _dictionaryLibrary._partIndexDictionary.ToDictionary(
                    entry => entry.Key.ToString(),
                    entry => {
                        var partDict = _dictionaryLibrary._availablePartDictionary[entry.Key];
                        int index = entry.Value;
                        return partDict.Keys.ElementAt(index); // get the part's name
                    }
                )
            };

            ES3.Save(saveKey, data, settings);
            Debug.Log("✅ Character saved!");
        }


        public void OpenThankyouPanel()
        {
            ThankYouForTestingMyGamePanel.SetActive(true);
        }

        public void BackToMainMenu()
        {
            PlayerPrefs.SetInt("ToGameMode", 1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
        }
    }
}
