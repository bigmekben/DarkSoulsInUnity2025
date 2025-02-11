using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class WeaponInventorySlot : MonoBehaviour
    {
        PlayerInventory playerInventory;
        WeaponSlotManager weaponSlotManager;
        UIManager uiManager;
        public Image icon;
        WeaponItem item;

        private void Awake()
        {
            playerInventory = FindFirstObjectByType<PlayerInventory>();
            weaponSlotManager = FindFirstObjectByType<WeaponSlotManager>();
            uiManager = FindFirstObjectByType<UIManager>();
        }
        public void AddItem(WeaponItem newItem)
        {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        public void ClearInventorySlot()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void EquipThisItem()
        {
            if(uiManager.rightHandSlot01Selected)
            {
                if (playerInventory.weaponsInRightHandSlots[0] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
                }
                playerInventory.weaponsInRightHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.rightHandSlot02Selected)
            {
                if (playerInventory.weaponsInRightHandSlots[1] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[1]);
                }
                playerInventory.weaponsInRightHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.rightHandSlot03Selected)
            {
                if (playerInventory.weaponsInRightHandSlots[2] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[2]);
                }
                playerInventory.weaponsInRightHandSlots[2] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.rightHandSlot04Selected)
            {
                if (playerInventory.weaponsInRightHandSlots[3] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[3]);
                }
                playerInventory.weaponsInRightHandSlots[3] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.leftHandSlot01Selected)
            {
                if (playerInventory.weaponsInLeftHandSlots[0] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[0]);
                }
                playerInventory.weaponsInLeftHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.leftHandSlot02Selected)
            {
                if (playerInventory.weaponsInLeftHandSlots[1] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[1]);
                }
                playerInventory.weaponsInLeftHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.leftHandSlot03Selected)
            {
                if (playerInventory.weaponsInLeftHandSlots[2] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[2]);
                }
                playerInventory.weaponsInLeftHandSlots[2] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if (uiManager.leftHandSlot04Selected)
            {
                if (playerInventory.weaponsInLeftHandSlots[3] != null)
                {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[3]);
                }
                playerInventory.weaponsInLeftHandSlots[3] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            playerInventory.rightWeapon = playerInventory.weaponsInRightHandSlots[playerInventory.currentRightWeaponIndex] ?? playerInventory.unarmedWeapon;
            playerInventory.leftWeapon = playerInventory.weaponsInLeftHandSlots[playerInventory.currentLeftWeaponIndex] ?? playerInventory.unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);

            uiManager.equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
            uiManager.ResetAllSelectedSlots();
        }
    }
}
