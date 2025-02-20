using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class HandEquipmentSlotUI : MonoBehaviour
    {
        UIManager uiManager;
        public Image icon;
        WeaponItem weapon;
        public bool rightHandSlot01;
        public bool rightHandSlot02;
        public bool rightHandSlot03;
        public bool rightHandSlot04;
        public bool leftHandSlot01;
        public bool leftHandSlot02;
        public bool leftHandSlot03;
        public bool leftHandSlot04;

        private void Awake()
        {
            uiManager = FindFirstObjectByType<UIManager>();
        }

        public void AddItem(WeaponItem newWeapon)
        {
            if(newWeapon != null)
            {
                weapon = newWeapon;
                icon.enabled = true;
                icon.sprite = weapon.itemIcon;
                gameObject.SetActive(true);
            }
        }

        public void ClearItem()
        {
            weapon = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }
 
        public void SelectThisSlot()
        {
            if(rightHandSlot01)
            {
                uiManager.rightHandSlot01Selected = true;
            }
            else if (rightHandSlot02)
            {
                uiManager.rightHandSlot02Selected = true;
            }
            else if (rightHandSlot03)
            {
                uiManager.rightHandSlot03Selected = true;
            }
            else if (rightHandSlot04)
            {
                uiManager.rightHandSlot04Selected = true;
            }
            else if (leftHandSlot01)
            {
                uiManager.leftHandSlot01Selected = true;
            }
            else if (leftHandSlot02)
            {
                uiManager.leftHandSlot02Selected = true;
            }
            else if (leftHandSlot03)
            {
                uiManager.leftHandSlot03Selected = true;
            }
            else if (leftHandSlot04)
            {
                uiManager.leftHandSlot04Selected = true;
            }
        }
    }
}
