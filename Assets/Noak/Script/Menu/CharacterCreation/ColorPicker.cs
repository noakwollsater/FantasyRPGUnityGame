using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Unity.FantasyKingdom
{
    public class ColorPicker : CharacterCreation
    {
        private List<SidekickColorProperty> _allColorProperties;

        void Start()
        {
            LazyInit();  // Ensure everything is initialized before running
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

            if (_allColorProperties == null || _allColorProperties.Count == 0)
            {
                _allColorProperties = SidekickColorProperty.GetAll(_dbManager);
                if (_allColorProperties == null || _allColorProperties.Count == 0)
                {
                    Debug.LogError("Failed to load color properties.");
                    return;
                }
            }
        }

        public void ChangeSkinColor(Image image)
        {
            if (image == null)
            {
                Debug.LogError("ChangeSkinColor: Image is null.");
                return;
            }

            LazyInit(); // Ensure everything is initialized before applying color

            Color newColor = image.color;
            ApplyColorChange(newColor, "skin");
        }

        public void ChangeOutfitColor(Image image)
        {
            if (image == null)
            {
                Debug.LogError("ChangeOutfitColor: Image is null.");
                return;
            }

            LazyInit(); // Ensure everything is initialized before applying color

            Color newColor = image.color;
            ApplyColorChange(newColor, "outfit");
        }

        private void ApplyColorChange(Color color, string propertyKeyword)
        {
            LazyInit(); // Ensure everything is initialized before applying color

            if (_sidekickRuntime == null)
            {
                Debug.LogError("ApplyColorChange: SidekickRuntime is not initialized.");
                return;
            }

            if (_allColorProperties == null || _allColorProperties.Count == 0)
            {
                Debug.LogError($"ApplyColorChange: No color properties found for '{propertyKeyword}'.");
                return;
            }

            List<SidekickColorProperty> selectedProperties = _allColorProperties.FindAll(prop => prop.Name.ToLower().Contains(propertyKeyword));

            if (selectedProperties.Count == 0)
            {
                Debug.LogError($"No color properties found for '{propertyKeyword}'.");
                return;
            }

            foreach (SidekickColorProperty property in selectedProperties)
            {
                SidekickColorRow row = new SidekickColorRow
                {
                    ColorProperty = property,
                    MainColor = ColorUtility.ToHtmlStringRGB(color),
                };

                _sidekickRuntime.UpdateColor(ColorType.MainColor, row);
            }

            Debug.Log($"Applied {propertyKeyword} color: {color}");
        }
    }
}
