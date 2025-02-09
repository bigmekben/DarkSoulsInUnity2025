using UnityEngine;

namespace SG
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;

        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        public WeaponItem unarmedWeapon;

        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[1];
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[1];

        public int currentRightWeaponIndex = -1;
        public int currentLeftWeaponIndex = -1;

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start()
        {
            rightWeapon = unarmedWeapon;
            leftWeapon = unarmedWeapon;
        }

        public void ChangeRightWeapon()
        {
            currentRightWeaponIndex++;
            // code from tutorial, needs to be refactored:
            if(currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }
            else if (currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] == null)
            {
                currentRightWeaponIndex++;
            }
            else if(currentRightWeaponIndex == 1 && weaponsInRightHandSlots[1] != null)
            {
                rightWeapon = weaponsInRightHandSlots[1];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }
            else
            {
                currentRightWeaponIndex++;
            }

            if(currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1)
            {
                currentRightWeaponIndex = -1;
                rightWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
            }
        }
    }
}
