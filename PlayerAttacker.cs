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
            int cost = (int)((float)(weapon.baseStamina) * weapon.lightAttackMultiplier); // to do: move cost property to weapon
            if(cost < playerStats.currentStamina)
            {
                weaponSlotManager.attackingWeapon = weapon;
                animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
                lastAttack = weapon.OH_Light_Attack_1;
            }
            else
            {
                // don't prevent further action while animating:
                animatorHandler.PlayTargetAnimation("ShakeHeadNo", false);
                // to do: animator.CrossFade() using avatar mask, so player can still walk around during head-shake movement and look natural
                Debug.Log($"This move costs {cost} stamina points, but you only have {playerStats.currentStamina}.");
            }
        }

        public void HandleHeavyAttack(WeaponItem weapon) 
        {
            int cost = (int)((float)weapon.baseStamina * weapon.heavyAttackMultiplier);
            if(cost < playerStats.currentStamina)
            {
                weaponSlotManager.attackingWeapon = weapon;
                animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
                lastAttack = weapon.OH_Heavy_Attack_1;
            }
            else
            {
                // don't prevent further action while animating:
                animatorHandler.PlayTargetAnimation("ShakeHeadNo", false);
                // to do: animator.CrossFade() using avatar mask, so player can still walk around during head-shake movement and look natural
                Debug.Log($"This move costs {cost} stamina points, but you only have {playerStats.currentStamina}.");
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
