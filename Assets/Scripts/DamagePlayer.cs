using UnityEngine;

namespace DarkSouls
{
    public class DamagePlayer : MonoBehaviour
    {
        private int damage = 25;

        /// <summary>
        /// This will induce damage onto the player reducing thier health
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null) { playerStats.TakeDamage(damage); }
        }
    }
}