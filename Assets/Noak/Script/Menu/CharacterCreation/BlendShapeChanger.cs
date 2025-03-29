using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class BlendShapeChanger : CharacterCreation, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Sprite[] genderIcons;
        [SerializeField] Image genderIcon;

        [SerializeField] Sprite[] BodySizes;
        [SerializeField] Image BodySizeImage;
        [SerializeField] Button bodySizeButton;

        private bool isMale = false;
        private int currentBodySizeIndex = 0;

        public float DefaultMuscle = 33f;
        public float DefaultSkinny = 33f;
        public float DefaultFat = 33f;

        void Start()
        {
            UpdateImage();
            LazyInit();
            DefaultValues();

            if (bodySizeButton != null)
            {
                EventTrigger trigger = bodySizeButton.gameObject.GetComponent<EventTrigger>();
                if (trigger == null) trigger = bodySizeButton.gameObject.AddComponent<EventTrigger>();

                AddEventTrigger(trigger, EventTriggerType.PointerEnter, OnHoverEnter);
                AddEventTrigger(trigger, EventTriggerType.PointerExit, OnHoverExit);
            }
        }

        private void DefaultValues()
        {
            Debug.Log("Resetting character to default size...");

            // Återställ blendshapes till defaultvärden
            _sidekickRuntime.MusclesBlendValue = DefaultMuscle;
            _sidekickRuntime.BodySizeSkinnyBlendValue = DefaultSkinny;
            _sidekickRuntime.BodySizeHeavyBlendValue = DefaultFat;

            _dictionaryLibrary.BodySizeSkinnyBlendValue = DefaultSkinny;
            _dictionaryLibrary.BodySizeHeavyBlendValue = DefaultFat;
            _dictionaryLibrary.MusclesBlendValue = DefaultMuscle;
        }


        private void LazyInit()
        {
            if (_dbManager == null)
            {
                _dbManager = new DatabaseManager();
                var connection = _dbManager.GetDbConnection(); // <-- This actually initializes the connection
                if (connection == null)
                {
                    Debug.LogError("Database connection failed.");
                    return;
                }
            }


            if (_sidekickRuntime == null)
            {
                GameObject model = Resources.Load<GameObject>("Meshes/SK_BaseModel");
                Material material = Resources.Load<Material>("Materials/M_BaseMaterial");

                _sidekickRuntime = new SidekickRuntime(model, material, null, _dbManager);
                if (_sidekickRuntime == null)
                {
                    Debug.LogError("SidekickRuntime failed to initialize.");
                    return;
                }
            }

            if (_dictionaryLibrary == null)
            {
                _dictionaryLibrary = new DictionaryLibrary();
                _dictionaryLibrary._partLibrary = _sidekickRuntime.PartLibrary;
            }
        }

        public void UpdateBodyComposition()
        {
            if (_sidekickRuntime != null)
            {
                float muscle = _sidekickRuntime.MusclesBlendValue / 100f;
                float skinny = _sidekickRuntime.BodySizeSkinnyBlendValue / 100f;
                float fat = _sidekickRuntime.BodySizeHeavyBlendValue / 100f;

                Race selectedRace = RaceDatabase.Instance.GetRace(RaceSelectionUI.SelectedRace);
                if (selectedRace == null)
                {
                    Debug.LogError($"Race '{RaceSelectionUI.SelectedRace}' not found!");
                    return;
                }

                // Skapa en separat kopia för modifierade stats
                AttributeSet baseAttributes = new AttributeSet(
                    selectedRace.BaseAttributes.Strength + selectedRace.RacialBonuses.Strength,
                    selectedRace.BaseAttributes.Dexterity + selectedRace.RacialBonuses.Dexterity,
                    selectedRace.BaseAttributes.Constitution + selectedRace.RacialBonuses.Constitution,
                    selectedRace.BaseAttributes.Intelligence + selectedRace.RacialBonuses.Intelligence,
                    selectedRace.BaseAttributes.Wisdom + selectedRace.RacialBonuses.Wisdom,
                    selectedRace.BaseAttributes.Charisma + selectedRace.RacialBonuses.Charisma
                );

                // Modifierade stats baserat på storlek
                AttributeSet modifiedAttributes = new AttributeSet(
                    baseAttributes.Strength, baseAttributes.Dexterity, baseAttributes.Constitution,
                    baseAttributes.Intelligence, baseAttributes.Wisdom, baseAttributes.Charisma
                );

                modifiedAttributes.ApplyBodyCompositionModifiers(muscle, skinny, fat);

                // Uppdatera UI:t med separata värden
                RaceSelectionUI.Instance.UpdateRaceStats(RaceSelectionUI.SelectedRace, modifiedAttributes);

                UpdateModel();
            }
        }


        public void SetGender()
        {
            if (_sidekickRuntime == null)
            {
                Debug.LogError("SetGender: SidekickRuntime is not initialized.");
                return;
            }

            isMale = !isMale;

            _sidekickRuntime.BodyTypeBlendValue = isMale ? 100 : 0; // <-- Updates gender
            _dictionaryLibrary.BodyTypeBlendValue = _sidekickRuntime.BodyTypeBlendValue;

            UpdateImage();
            UpdateModel();
        }

        private void UpdateImage()
        {
            Debug.Log($"isMale: {isMale}, Setting sprite to: {(isMale ? "Male" : "Female")}");
            if (genderIcons.Length >= 2)
            {
                genderIcon.sprite = isMale ? genderIcons[0] : genderIcons[1];
                genderIcon.color = isMale ? new Color(0.5f, 0.7f, 1f) : new Color(1f, 0.5f, 0.7f);
            }
            else
            {
                Debug.LogError("Gender icons array does not contain enough elements.");
            }
        }

        private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
            entry.callback.AddListener((data) => action.Invoke(data));
            trigger.triggers.Add(entry);
        }

        private void OnHoverEnter(BaseEventData data)
        {
            if (BodySizes.Length > 1)
            {
                int nextIndex = (currentBodySizeIndex + 1) % BodySizes.Length;
                BodySizeImage.sprite = BodySizes[nextIndex];
            }
        }

        private void OnHoverExit(BaseEventData data)
        {
            if (BodySizes.Length > 0)
            {
                BodySizeImage.sprite = BodySizes[currentBodySizeIndex];
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!eventData.pointerEnter.transform.IsChildOf(bodySizeButton.transform))
                return;
            OnHoverEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!eventData.pointerEnter.transform.IsChildOf(bodySizeButton.transform))
                return;
            OnHoverExit(eventData);
        }
    }
}
