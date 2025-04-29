using UnityEngine;

namespace DarkSouls
{
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;

        int currentWeaponDamage = 25;

        /// <summary>
        /// This enables the collider to trigger another function when collided with, in this case a player
        /// </summary>
        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void EnableDamageCollider() { damageCollider.enabled = true; }
        public void DisableDamageCollider() { damageCollider.enabled = false; }

        /// <summary>
        /// This function will add functionality when the item is collided with. 
        /// The player will take damage, reducing thier health.
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Hittable")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();
                if (playerStats != null) { playerStats.TakeDamage(currentWeaponDamage); }
            }
        }
    }
}
