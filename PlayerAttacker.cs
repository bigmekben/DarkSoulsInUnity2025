using System;
using UnityEngine;
namespace SG
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }

        public void HandleLightAttack(WeaponItem weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
            animatorHandler.SetBool("lightAttacking");
        }

        public void HandleHeavyAttack(WeaponItem weapon) 
        { 
            animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_attack_1 , true);
            animatorHandler.SetBool("heavyAttacking");
        }

        internal bool LightAttackBusy() => animatorHandler.GetBool("lightAttacking");

        internal bool HeavyAttackBusy() => animatorHandler.GetBool("heavyAttacking");
    }
}
