using System;
using UnityEngine;
namespace SG
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;
        InputHandler inputHandler;
        WeaponSlotManager weaponSlotManager;
        PlayerStats playerStats;
        public string lastAttack;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            inputHandler = GetComponent<InputHandler>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            playerStats = GetComponent<PlayerStats>();
        }

        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if(inputHandler.comboFlag)
            {
                animatorHandler.anim.SetBool("canDoCombo", false);
                if (lastAttack == weapon.OH_Light_Attack_1)
                {
                    animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                }
            }
        }

        public void HandleLightAttack(WeaponItem weapon)
        {
            if(weapon.baseStamina * weapon.lightAttackMultiplier < playerStats.currentStamina)
            {
                weaponSlotManager.attackingWeapon = weapon;
                animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
                lastAttack = weapon.OH_Light_Attack_1;
            }
        }

        public void HandleHeavyAttack(WeaponItem weapon) 
        { 
            if(weapon.baseStamina * weapon.heavyAttackMultiplier < playerStats.currentStamina)
            {
                weaponSlotManager.attackingWeapon = weapon;
                animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
                lastAttack = weapon.OH_Heavy_Attack_1;
            }
        }


        // temp: equip animations here.  Later: make PlayerEquipper, or something, to move these there.
        public void EquipRightWaist()
        {
            // for now: make it so you can't move while switching weapons.
            // to do: allow to continue walking or sprinting, but not rolling, attacking, etc.
            // for this, would need to split the "interacting" flag into multiple flags.
            animatorHandler.PlayTargetAnimation("Equip_Right_Waist", true);
        }
    }
}
