using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace DarkSouls
{
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;

        int currentWeaponDamage = 25;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void EnableDamageCollider() { damageCollider.enabled = true; }
        public void DisableDamageCollider() { damageCollider.enabled = false; }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Hittable")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();

                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentWeaponDamage);
                }
            }
        }
    }
}
