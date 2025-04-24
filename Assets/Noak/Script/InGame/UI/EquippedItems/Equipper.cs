using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Unity.FantasyKingdom
{
    public class Equipper : MonoBehaviour
    {
        [SerializeField] private GameObject weaponWheelPanel;
        [SerializeField] private GameObject equippedItem;
        [SerializeField] private List<WeaponWheelSlot> slots;

        private Sprite equippedIcon;
        private Sprite selectedItem;

        [SerializeField] private Image equippedImage;

        void Start()
        {
            initializeList();
        }

        public void initializeList()
        {
            foreach (var slot in slots)
            {
                slot.Initialize(this);
            }
        }

        public void SelectItemFromSlot(Sprite sprite)
        {
            selectedItem = sprite;
            EquipSelectedItem();
        }

        private void EquipSelectedItem()
        {
            equippedIcon = selectedItem;

            if (equippedImage != null)
            {
                equippedImage.sprite = equippedIcon;
                Debug.Log("Equipped: " + equippedIcon.name);
            }
        }

    }
}
