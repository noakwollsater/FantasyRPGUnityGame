using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class BlendShapeChanger : CharacterCreation
    {
        [SerializeField] Sprite[] genderIcons;
        [SerializeField] Image genderIcon;

        private bool isMale = false;

        void Start()
        {
            UpdateImage();
            LazyInit();
        }

        private void LazyInit()
        {
            if (_dbManager == null)
            {
                _dbManager = new DatabaseManager();
                if (_dbManager.GetCurrentDbConnection() == null)
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
                _sidekickRuntime.BodySizeSkinnyBlendValue = _dictionaryLibrary.BodySizeSkinnyBlendValue;
                _sidekickRuntime.BodySizeHeavyBlendValue = _dictionaryLibrary.BodySizeHeavyBlendValue;
                _sidekickRuntime.MusclesBlendValue = _dictionaryLibrary.MusclesBlendValue;

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

    }
}
