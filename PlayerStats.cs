using System;
using UnityEngine;
namespace SG
{
    public class PlayerStats : MonoBehaviour
    {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        public int staminaLevel = 10;
        public int maxStamina;
        public int currentStamina;

        public HealthBar healthBar;
        public StaminaBar staminaBar;
        AnimatorHandler animatorHandler;

        public float staminaRegenTimer = 0f;
        public float staminaRegenDelay = 2f;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            healthBar = FindFirstObjectByType<HealthBar>();
            staminaBar = FindFirstObjectByType<StaminaBar>();
        }

        void Start()
        {
            currentHealth = SetMaxHealthFromHealthLevel();
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);

            currentStamina = SetMaxStaminaFromStaminaLevel();
            staminaBar.SetMaxStamina(maxStamina);
            staminaBar.SetCurrentStamina(currentStamina);
        }

        public void Update()
        {
            if (staminaRegenTimer < staminaRegenDelay)
            {
                staminaRegenTimer += Time.deltaTime;
            }
            else
            {
                RegenStamina();
            }
        }

        private void RegenStamina()
        {
            if(currentStamina < maxStamina)
            {
                currentStamina++;
                staminaBar.SetCurrentStamina(currentStamina);
            }
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        private int SetMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);
            if(currentHealth <= 0)
            {
                currentHealth = 0;
                animatorHandler.PlayTargetAnimation("Dead_01", true);
                // handle player death
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Damage_01", true);
            }
        }

        public void TakeStaminaDamage(int damage)
        {
            currentStamina -= damage;
            staminaBar.SetCurrentStamina(currentStamina);
            staminaRegenTimer = 0;
        }
    }
}
