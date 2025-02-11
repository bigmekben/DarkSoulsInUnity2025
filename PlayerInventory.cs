using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SG
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;

        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        public WeaponItem unarmedWeapon;

        public WeaponItem[] weaponsInRightHandSlots;
        public WeaponItem[] weaponsInLeftHandSlots;

        public int currentRightWeaponIndex = -1;
        public int currentLeftWeaponIndex = -1;

        public List<WeaponItem> weaponsInventory;

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            if (weaponsInRightHandSlots == null)
            {
                weaponsInRightHandSlots = new WeaponItem[0];
            }
            if (weaponsInLeftHandSlots == null)
            {
                weaponsInLeftHandSlots = new WeaponItem[0];
            }
        }

        private void Start()
        {
            rightWeapon = weaponsInRightHandSlots[0] ?? unarmedWeapon;
            leftWeapon = weaponsInLeftHandSlots[0] ?? unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }

        public void ChangeRightWeapon()
        {
            if(weaponsInRightHandSlots.Count(w=>w != null) > 0)
            {
                currentRightWeaponIndex++;
                if (currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1)
                {
                    currentRightWeaponIndex = -1;
                    rightWeapon = unarmedWeapon;
                    weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
                }
                else
                {
                    if (weaponsInRightHandSlots[currentRightWeaponIndex] != null)
                    {
                        rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                        weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
                    }
                    else
                    {
                        currentRightWeaponIndex++;
                    }
                }
            }
        }

        internal void ChangeLeftWeapon()
        {
            if(weaponsInLeftHandSlots.Count(w => w != null) > 0)
            {
                currentLeftWeaponIndex++;
                if (currentLeftWeaponIndex > weaponsInLeftHandSlots.Length - 1)
                {
                    currentLeftWeaponIndex = -1;
                    leftWeapon = unarmedWeapon;
                    weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
                }
                else
                {
                    if (weaponsInLeftHandSlots[currentLeftWeaponIndex] != null)
                    {
                        leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                        weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
                    }
                    else
                    {
                        currentLeftWeaponIndex++;
                    }
                }
            }
        }
    }
}
