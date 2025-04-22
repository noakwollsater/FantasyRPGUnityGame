using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;

namespace Unity.FantasyKingdom
{
    public class GetActiveColors : MonoBehaviour
    {
        public Dictionary<string, string> selectedColors = new();

        private SidekickRuntime _sidekickRuntime;

        public void GetColorsFromRuntime()
        {
            var model = Resources.Load<GameObject>("Meshes/SK_BaseModel");
            var material = Resources.Load<Material>("Materials/M_BaseMaterial");
            var db = new DatabaseManager();

            _sidekickRuntime = new SidekickRuntime(model, material, null, db);

            List<SidekickColorProperty> allProps = SidekickColorProperty.GetAll(db);

            selectedColors.Clear();

            foreach (var prop in allProps)
            {
                Color currentColor = material.GetColor("_BaseColor"); // Fallback if texture isn't used

                // Försök hämta färg från korrekt UV om det finns en textur
                Texture2D colorMap = material.GetTexture("_BaseColorMap") as Texture2D;
                if (colorMap != null)
                {
                    int u = Mathf.Clamp(prop.U * 2, 0, colorMap.width - 1);
                    int v = Mathf.Clamp(prop.V * 2, 0, colorMap.height - 1);
                    currentColor = colorMap.GetPixel(u, v);
                }

                string hex = ColorUtility.ToHtmlStringRGB(currentColor);
                selectedColors[prop.Name.ToLower()] = hex;
            }

            Debug.Log($"🎨 Hämtade {selectedColors.Count} färger från material.");
        }
    }
}
