using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

namespace Unity.FantasyKingdom
{
    public class ColorPicker : CharacterCreation, IPointerDownHandler, IDragHandler
    {
        private List<SidekickColorProperty> _allColorProperties;
        public Image colorWheelImage;
        public RectTransform colorWheelRect;
        public Color selectedColor;
        private Texture2D colorWheelTexture;
        private List<string> currentParts = new List<string> { "skin", "ear", "nose" };

        void Start()
        {
            LazyInit();  // Ensure everything is initialized before running
            if (colorWheelImage != null)
            {
                Texture2D originalTexture = colorWheelImage.sprite.texture;

                // Ensure the original texture is readable
                RenderTexture renderTex = RenderTexture.GetTemporary(
                    originalTexture.width,
                    originalTexture.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

                Graphics.Blit(originalTexture, renderTex);
                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = renderTex;

                colorWheelTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
                colorWheelTexture.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
                colorWheelTexture.Apply();

                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(renderTex);
            }


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

        public void SetCurrentParts(params string[] parts)
        {
            currentParts = parts.ToList();
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
                selectedColor = colorWheelTexture.GetPixelBilinear(normalized.x, normalized.y);
                ApplyColorChange(selectedColor, currentParts.ToArray());
            }
        }

        private void ApplyColorChange(Color color, params string[] propertyKeywords)
        {
            LazyInit();

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

        public void ChangeSkinColor() => SetCurrentParts("skin", "ear", "nose");
        public void ChangeEyeColor() => SetCurrentParts("eye color");
        public void ChangeHairColor() => SetCurrentParts("hair");
        public void ChangeEyebrowcolors() => SetCurrentParts("eyebrow");
        public void ChangeFacialhairColor() => SetCurrentParts("facial hair");
        public void ChangeFingernailColor() => SetCurrentParts("fingernails");
    }
}
