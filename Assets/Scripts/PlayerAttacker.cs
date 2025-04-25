using UnityEngine;

namespace DarkSouls
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;

        private void Awake() { animatorHandler = GetComponent<AnimatorHandler>(); }

        /// <summary>
        /// This function is responsible for executing animations pertaining to light attacks
        /// </summary>
        /// <param name="weapon"></param>
        public void HandleLightAttack(WeaponItem weapon) { animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true); }

        /// <summary>
        /// This function is responsible for executing animations pertaining to heavy attacks
        /// </summary>
        /// <param name="weapon"></param>
        public void HandleHeavyAttack(WeaponItem weapon) { animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true); }
    }
}