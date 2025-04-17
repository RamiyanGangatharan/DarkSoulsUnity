using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Manages the player's equipped weapons and interfaces with the WeaponSlotManager
    /// to load them into the appropriate slots at the start of the game.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        private WeaponSlotManager weaponSlotManager;

        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        /// <summary>
        /// Initializes references to required components.
        /// </summary>
        private void Awake() { weaponSlotManager = GetComponentInChildren<WeaponSlotManager>(); }

        /// <summary>
        /// Loads the equipped weapons into their respective slots when the game starts.
        /// </summary>
        private void Start()
        {
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }
    }
}
