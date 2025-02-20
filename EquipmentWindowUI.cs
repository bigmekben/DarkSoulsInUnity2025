using UnityEngine;
namespace SG
{
    public class EquipmentWindowUI : MonoBehaviour
    {
        public bool rightHandSlot01Selected;
        public bool rightHandSlot02Selected;
        public bool rightHandSlot03Selected;
        public bool rightHandSlot04Selected;
        public bool leftHandSlot01Selected;
        public bool leftHandSlot02Selected;
        public bool leftHandSlot03Selected;
        public bool leftHandSlot04Selected;

        public HandEquipmentSlotUI[] handEquipmentSlotUI;

        private void Start()
        {
        }

        public void LoadWeaponsOnEquipmentScreen(PlayerInventory playerInventory)
        {
            for (int i = 0; i < handEquipmentSlotUI.Length; i++)
            {
                if (handEquipmentSlotUI[i].rightHandSlot01)
                {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[0]);
                }
                else if (handEquipmentSlotUI[i].rightHandSlot02)
                {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[1]);
                }
                else if (handEquipmentSlotUI[i].rightHandSlot03)
                {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[2]);
                }
                else if (handEquipmentSlotUI[i].rightHandSlot04)
                {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[3]);
                }
                else if (handEquipmentSlotUI[i].leftHandSlot01)
                {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[0]);
                }
                else if (handEquipmentSlotUI[i].leftHandSlot02)
                {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[1]);
                }
                else if (handEquipmentSlotUI[i].leftHandSlot03)
                {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[2]);
                }
                else if (handEquipmentSlotUI[i].leftHandSlot04)
                {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[3]);
                }
            }
        }

        private void ResetAllSelected()
        {
            rightHandSlot01Selected = false;
            rightHandSlot02Selected = false;
            rightHandSlot03Selected = false;
            rightHandSlot04Selected = false;
            leftHandSlot01Selected = false;
            leftHandSlot02Selected = false;
            leftHandSlot03Selected = false;
            leftHandSlot04Selected = false;
        }

        public void SelectRightHandSlot01()
        {
            ResetAllSelected();
            rightHandSlot01Selected = true;
        }

        public void SelectRightHandSlot02()
        {
            ResetAllSelected();
            rightHandSlot02Selected = true;
        }

        public void SelectRightHandSlot03()
        {
            ResetAllSelected();
            rightHandSlot03Selected = true;
        }

        public void SelectRightHandSlot04()
        {
            ResetAllSelected();
            rightHandSlot04Selected = true;
        }

        public void SelectLeftHandSlot01()
        {
            ResetAllSelected();
            leftHandSlot01Selected = true;
        }

        public void SelectLeftHandSlot02()
        {
            ResetAllSelected();
            leftHandSlot02Selected = true;
        }

        public void SelectLeftHandSlot03()
        {
            ResetAllSelected();
            leftHandSlot03Selected = true;
        }

        public void SelectLeftHandSlot04()
        {
            ResetAllSelected();
            leftHandSlot04Selected = true;
        }
    }
}
