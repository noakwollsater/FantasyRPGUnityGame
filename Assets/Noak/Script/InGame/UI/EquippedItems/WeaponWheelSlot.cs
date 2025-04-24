using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class WeaponWheelSlot : MonoBehaviour
    {
        public Image icon;
        private Equipper equipper;

        public void Initialize(Equipper equipper)
        {
            this.equipper = equipper;
        }

        public void SelectThisSlot()
        {
            if (icon != null && equipper != null)
            {
                equipper.SelectItemFromSlot(icon.sprite);
            }
        }
    }
}
