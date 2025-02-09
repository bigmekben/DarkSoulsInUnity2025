using UnityEngine;
namespace SG
{
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;
        public int currentWeaponDamage = 25; // to do: get damage from a dictionary or something

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if(collision.tag == "Player")
            {
                var stats = collision.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.TakeDamage(currentWeaponDamage);
                }
            }
            if (collision.tag == "NPC")
            {
                var stats = collision.GetComponent<NPCStats>();
                if (stats != null)
                {
                    stats.TakeDamage(currentWeaponDamage);
                }
            }
        }
    }
}
