using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;

namespace Unity.FantasyKingdom
{
    public class DynamicColorWheel : CharacterCreation, IPointerDownHandler, IDragHandler
    {
        public RawImage colorWheelImage;
        public RectTransform colorWheelRect;
        public int textureSize = 256;
        private string currentMode = "RGB Spectrum";
        private Texture2D generatedTexture;
        private List<string> currentParts = new List<string> { "skin", "ear", "nose" };
        private Color selectedColor;

        private List<SidekickColorProperty> _allColorProperties;

        void Start()
        {
            LazyInit();
            GenerateAndSetColorWheel();
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
            }

            if (_dictionaryLibrary == null)
            {
                _dictionaryLibrary = new DictionaryLibrary();
                _dictionaryLibrary._partLibrary = _sidekickRuntime.PartLibrary;
            }

            if (_allColorProperties == null || _allColorProperties.Count == 0)
            {
                _allColorProperties = SidekickColorProperty.GetAll(_dbManager);
            }
        }

        public void SetColorMode(string mode)
        {
            currentMode = mode;
            GenerateAndSetColorWheel();
        }

        private void GenerateAndSetColorWheel()
        {
            generatedTexture = GenerateColorWheel(textureSize, currentMode);
            colorWheelImage.texture = generatedTexture;
        }

        private Texture2D GenerateColorWheel(int size, string mode)
        {
            Texture2D texture = new Texture2D(size, size);
            Vector2 center = new Vector2(size / 2f, size / 2f);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Vector2 pos = new Vector2(x, y) - center;
                    float angle = Mathf.Atan2(pos.y, pos.x);
                    float radius = pos.magnitude / (size / 2f);

                    if (radius > 1)
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        texture.SetPixel(x, y, mode == "Human Skin Tones" ? AngleToSkinTone(radius) : AngleToRGB(angle));
                    }
                }
            }

            texture.Apply();
            return texture;
        }

        private Color AngleToRGB(float angle)
        {
            float r = Mathf.Cos(angle) * 0.5f + 0.5f;
            float g = Mathf.Cos(angle - 2 * Mathf.PI / 3) * 0.5f + 0.5f;
            float b = Mathf.Cos(angle - 4 * Mathf.PI / 3) * 0.5f + 0.5f;
            return new Color(r, g, b, 1f);
        }

        private Color AngleToSkinTone(float intensity)
        {
            Color[] skinTones = new Color[]
            {
                new Color(1.0f, 0.85f, 0.7f),
                new Color(0.96f, 0.75f, 0.56f),
                new Color(0.87f, 0.65f, 0.46f),
                new Color(0.76f, 0.55f, 0.37f),
                new Color(0.66f, 0.44f, 0.3f),
                new Color(0.55f, 0.34f, 0.23f),
                new Color(0.4f, 0.26f, 0.18f),
            };

            float index = intensity * (skinTones.Length - 1);
            int lowerIndex = Mathf.FloorToInt(index);
            int upperIndex = Mathf.Clamp(lowerIndex + 1, 0, skinTones.Length - 1);
            float blendFactor = index - lowerIndex;

            return Color.Lerp(skinTones[lowerIndex], skinTones[upperIndex], blendFactor);
        }

        public void OnPointerDown(PointerEventData eventData) => UpdateColor(eventData);
        public void OnDrag(PointerEventData eventData) => UpdateColor(eventData);

        private void UpdateColor(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                colorWheelRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

            Vector2 normalized = new Vector2(
                (localPoint.x / colorWheelRect.rect.width) + 0.5f,
                (localPoint.y / colorWheelRect.rect.height) + 0.5f
            );

            if (normalized.x >= 0 && normalized.x <= 1 && normalized.y >= 0 && normalized.y <= 1)
            {
                selectedColor = generatedTexture.GetPixelBilinear(normalized.x, normalized.y);
                ApplyColorChange(selectedColor, currentParts.ToArray());
            }
        }

        private void ApplyColorChange(Color color, params string[] propertyKeywords)
        {
            if (_sidekickRuntime == null || _allColorProperties == null || _allColorProperties.Count == 0)
            {
                Debug.LogError("ApplyColorChange: SidekickRuntime is not initialized or no color properties found.");
                return;
            }

            List<SidekickColorProperty> selectedProperties = _allColorProperties
                .Where(prop => propertyKeywords.Any(keyword => prop.Name.ToLower().Contains(keyword.ToLower())))
                .ToList();

            if (selectedProperties.Count == 0)
            {
                Debug.LogError($"No color properties found for '{string.Join(", ", propertyKeywords)}'.");
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

            Debug.Log($"Applied color {color} to properties: {string.Join(", ", propertyKeywords)}");
        }

        public void SetCurrentParts(params string[] parts)
        {
            currentParts = parts.ToList();
            Debug.Log($"Current parts set to: {string.Join(", ", currentParts)}");
        }

        public void ChangeSkinColor() => SetColorMode("Human Skin Tones");
        public void ChangeEyeColor() => SetCurrentParts("eye color");
        public void ChangeHairColor() => SetCurrentParts("hair");
        public void ChangeEyebrowColor() => SetCurrentParts("eyebrow");
        public void ChangeFacialHairColor() => SetCurrentParts("facial hair");
        public void ChangeFingernailColor() => SetCurrentParts("fingernails");
    }
}
