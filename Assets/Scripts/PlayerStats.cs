using UnityEngine;

namespace DarkSouls
{
    public class PlayerStats : MonoBehaviour
    {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        public HealthBar healthBar;

        AnimatorHandler animatorHandler;
        PlayerManager playerManager;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponentInChildren<PlayerManager>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            maxHealth = SetMaximumHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaximumHealth(maxHealth);
        }

        /// <summary>
        /// This will calculate the max health a player can have
        /// </summary>
        /// <returns>The maximum value for health in a healthbar</returns>
        private int SetMaximumHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        /// <summary>
        /// This function will reduce your health when you take damage.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage) 
        { 
            currentHealth = currentHealth - damage; 
            healthBar.SetCurrentHealth(currentHealth);
            animatorHandler.PlayTargetAnimation("GetHit", false);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animatorHandler.PlayTargetAnimation("Death", true);
                playerManager.isDead = true;

                //HANDLE DEATH
            }
        }
    }

}
