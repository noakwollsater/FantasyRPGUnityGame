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
                            "Cloth Tones" => AngleOToClothTone(radius, angle),
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
                new Color(0.72f, 0.71f, 0.58f),
                new Color(0.63f, 0.61f, 0.44f),
                new Color(0.533f, 0.509f, 0.298f),
                new Color(0.43f, 0.41f, 0.24f),
                new Color(0.32f, 0.31f, 0.18f),
                new Color(0.27f, 0.25f, 0.15f)

            };

            return GetBlendedColor(goblinTones, intensity);
        }

        private Color AngleOToClothTone(float intensity, float angle)
        {
            Color[] redTones =
            {
        new Color(0.75f, 0.2f, 0.2f),  // Brick Red
        new Color(0.6f, 0.1f, 0.1f),   // Deep Red
        new Color(0.5f, 0.05f, 0.05f)  // Dark Crimson
    };

            Color[] brownTones =
            {
        new Color(0.6f, 0.4f, 0.2f),   // Warm Brown
        new Color(0.5f, 0.3f, 0.1f),   // Leather Brown
        new Color(0.35f, 0.2f, 0.1f)   // Dark Brown
    };

            Color[] yellowTones =
            {
        new Color(0.9f, 0.8f, 0.3f),   // Golden Yellow
        new Color(0.8f, 0.7f, 0.2f),   // Mustard Yellow
        new Color(0.7f, 0.6f, 0.1f)    // Earthy Yellow
    };

            Color[] greenTones =
            {
        new Color(0.3f, 0.5f, 0.2f),   // Olive Green
        new Color(0.2f, 0.4f, 0.15f),  // Moss Green
        new Color(0.15f, 0.3f, 0.1f)   // Forest Green
    };

            Color[] blueTones =
            {
        new Color(0.2f, 0.3f, 0.6f),   // Deep Blue
        new Color(0.3f, 0.4f, 0.7f),   // Denim Blue
        new Color(0.15f, 0.2f, 0.5f)   // Navy Blue
    };

            Color[] purpleTones =
            {
        new Color(0.6f, 0.4f, 0.7f),   // Muted Violet
        new Color(0.7f, 0.5f, 0.8f),   // Soft Purple
        new Color(0.5f, 0.3f, 0.6f)    // Dark Lavender
    };

            // **Fix: Normalize angle mapping for equal segment sizes**
            float normalizedAngle = (angle % (2 * Mathf.PI) + 2 * Mathf.PI) % (2 * Mathf.PI);
            normalizedAngle /= (2 * Mathf.PI);
            float sector = normalizedAngle * 6; // Map to 6 color segments
            int lowerIndex = Mathf.FloorToInt(sector) % 6;
            int upperIndex = (lowerIndex + 1) % 6;
            float blendFactor = sector - lowerIndex; // Fractional blending between two segments

            Color[] lowerColorSet = lowerIndex switch
            {
                0 => redTones,
                1 => brownTones,
                2 => yellowTones,
                3 => greenTones,
                4 => blueTones,
                _ => purpleTones
            };

            Color[] upperColorSet = upperIndex switch
            {
                0 => redTones,
                1 => brownTones,
                2 => yellowTones,
                3 => greenTones,
                4 => blueTones,
                _ => purpleTones
            };

            // **Fix: Ensure better blending between yellow-green and red-purple**
            if (lowerIndex == 5 && upperIndex == 0)
            {
                blendFactor = Mathf.SmoothStep(0f, 1f, blendFactor * 0.75f); // More gradual transition
            }


            Color blendedColor1 = GetBlendedColor(lowerColorSet, intensity);
            Color blendedColor2 = GetBlendedColor(upperColorSet, intensity);
            Color finalColor = Color.Lerp(blendedColor1, blendedColor2, blendFactor);

            // **Final Fix: Slightly increase contrast to avoid washed-out areas**
            float grayscale = Mathf.Clamp01(1 - intensity); // White in center, black near edges
            Color shadingBlend = Color.Lerp(Color.white, Color.black, intensity * 0.7f);
            finalColor = Color.Lerp(finalColor, shadingBlend, 0.15f); // Subtle contrast boost

            return finalColor;
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
        public void ChangeRGBColor() => SetColorMode("RGB Spectrum");
        public void ChangeClothColor() => SetColorMode("Cloth Tones");

        public void ChangeSkinColor() => SetCurrentParts("skin", "ear", "nose");
        public void ChangeEyeColor() => SetCurrentParts("edge","eye color");
        public void ChangeHairColor() => SetCurrentParts("hair 01");
        public void ChangeEyebrowColor() => SetCurrentParts("eyebrow");
        public void ChangeFacialHairColor() => SetCurrentParts("facial hair");
        public void ChangeOutfit01() => SetCurrentParts("outfit 01");
        public void ChangeOutfit02() => SetCurrentParts("outfit 02");
        public void Changestrap() => SetCurrentParts("strap");

    }
}
