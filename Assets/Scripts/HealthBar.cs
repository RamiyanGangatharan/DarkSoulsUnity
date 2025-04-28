using UnityEngine;
using UnityEngine.UI;

namespace DarkSouls
{
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;

        /// <summary>
        /// This will make sure that the maximum amount of health can be set at the start of the game.
        /// </summary>
        /// <param name="maxHealth"></param>
        public void SetMaximumHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        /// <summary>
        /// Health be a dynamic value so it is stored in a function.
        /// </summary>
        /// <param name="currentHealth"></param>
        public void SetCurrentHealth(int currentHealth) { slider.value = currentHealth; }
    }
}

