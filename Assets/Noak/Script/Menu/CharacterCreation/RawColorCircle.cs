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
        private List<string> currentParts = new List<string> { "skin", "ear", "nose" };
        private Color selectedColor;
        private Texture2D generatedTexture;
        private List<SidekickColorProperty> _allColorProperties;

        void Start()
        {
            Initialize();
            GenerateAndSetColorWheel();
            ChangeSkinTone();

        }

        private void Initialize()
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
                var model = Resources.Load<GameObject>("Meshes/SK_BaseModel");
                var material = Resources.Load<Material>("Materials/M_BaseMaterial");
                _sidekickRuntime = new SidekickRuntime(model, material, null, _dbManager);
            }

            if (_dictionaryLibrary == null)
            {
                _dictionaryLibrary = new DictionaryLibrary
                {
                    _partLibrary = _sidekickRuntime.PartLibrary
                };
            }

            _allColorProperties ??= SidekickColorProperty.GetAll(_dbManager);
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
                        texture.SetPixel(x, y, mode switch
                        {
                            "Human Skin Tones" => AngleToSkinTone(radius),
                            "Goblin Tones" => AngleToGoblinTone(radius),
                            _ => AngleToRGB(angle)
                        });
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
            Color[] skinTones =
            {
                new Color(1.0f, 0.85f, 0.7f),
                new Color(0.96f, 0.75f, 0.56f),
                new Color(0.87f, 0.65f, 0.46f),
                new Color(0.76f, 0.55f, 0.37f),
                new Color(0.66f, 0.44f, 0.3f),
                new Color(0.55f, 0.34f, 0.23f),
                new Color(0.4f, 0.26f, 0.18f)
            };

            return GetBlendedColor(skinTones, intensity);
        }

        private Color AngleToGoblinTone(float intensity)
        {
            Color[] goblinTones =
            {
                new Color(0.55f, 0.94f, 0.55f),  // Light Green
                new Color(0.36f, 0.78f, 0.36f),  // Medium Green
                new Color(0.24f, 0.64f, 0.24f),  // Dark Green
                new Color(0.18f, 0.50f, 0.18f),  // Deep Green
                new Color(0.10f, 0.36f, 0.10f)   // Shadowed Green
            };

            return GetBlendedColor(goblinTones, intensity);
        }

        private Color GetBlendedColor(Color[] tones, float intensity)
        {
            float index = intensity * (tones.Length - 1);
            int lowerIndex = Mathf.FloorToInt(index);
            int upperIndex = Mathf.Clamp(lowerIndex + 1, 0, tones.Length - 1);
            float blendFactor = index - lowerIndex;

            return Color.Lerp(tones[lowerIndex], tones[upperIndex], blendFactor);
        }

        public void OnPointerDown(PointerEventData eventData) => UpdateColor(eventData);
        public void OnDrag(PointerEventData eventData) => UpdateColor(eventData);

        private void UpdateColor(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                colorWheelRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint)) return;

            Vector2 normalized = new Vector2(
                (localPoint.x / colorWheelRect.rect.width) + 0.5f,
                (localPoint.y / colorWheelRect.rect.height) + 0.5f
            );

            if (normalized.x is < 0 or > 1 || normalized.y is < 0 or > 1) return;

            selectedColor = generatedTexture.GetPixelBilinear(normalized.x, normalized.y);
            ApplyColorChange(selectedColor, currentParts.ToArray());
        }

        private void ApplyColorChange(Color color, params string[] propertyKeywords)
        {
            if (_sidekickRuntime == null || _allColorProperties == null)
            {
                Debug.LogError("ApplyColorChange: SidekickRuntime or color properties are missing.");
                return;
            }

            var selectedProperties = _allColorProperties
                .Where(prop => propertyKeywords.Any(keyword => prop.Name.ToLower().Contains(keyword.ToLower())))
                .ToList();

            if (!selectedProperties.Any())
            {
                Debug.LogError($"No color properties found for '{string.Join(", ", propertyKeywords)}'.");
                return;
            }

            foreach (var property in selectedProperties)
            {
                _sidekickRuntime.UpdateColor(ColorType.MainColor, new SidekickColorRow
                {
                    ColorProperty = property,
                    MainColor = ColorUtility.ToHtmlStringRGB(color)
                });
            }

            Debug.Log($"Applied color {color} to {string.Join(", ", propertyKeywords)}");
        }

        private void ChangeSkinTone()
        {
            if (_dictionaryLibrary.selectedSpecies == "Human")
            {
                SetColorMode("Human Skin Tones");
            }
            else if (_dictionaryLibrary.selectedSpecies == "Goblin")
            {
                SetColorMode("Goblin Tones");
            }
        }

        public void SetCurrentParts(params string[] parts) => currentParts = parts.ToList();

        public void ChangeCharacterSkin() => ChangeSkinTone();
        //public void ChangeHumanSkinColor() => SetColorMode("Human Skin Tones");
        //public void ChangeGoblinSkinColor() => SetColorMode("Goblin Tones");
        public void ChangeRGBColor() => SetColorMode("RGB Spectrum");

        public void ChangeSkinColor() => SetCurrentParts("skin", "ear", "nose");
        public void ChangeEyeColor() => SetCurrentParts("eye");
        public void ChangeHairColor() => SetCurrentParts("hair");
        public void ChangeEyebrowColor() => SetCurrentParts("eyebrow");
        public void ChangeFacialHairColor() => SetCurrentParts("facial hair");
    }
}
