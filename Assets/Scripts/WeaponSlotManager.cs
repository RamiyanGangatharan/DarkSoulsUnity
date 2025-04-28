using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Manages weapon holder slots on the player character and handles loading
    /// weapon models into the appropriate hand slots.
    /// </summary>
    public class WeaponSlotManager : MonoBehaviour
    {
        WeaponHolderSlot leftHandSlot;
        WeaponHolderSlot rightHandSlot;

        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;

        /// <summary>
        /// Initializes weapon holder slot references by searching child components.
        /// </summary>
        private void Awake()
        {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();

            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
            {
                if (weaponSlot.isLeftHandSlot) { leftHandSlot = weaponSlot; }
                else if (weaponSlot.isRightHandSlot) { rightHandSlot = weaponSlot; }
            }
        }

        /// <summary>
        /// Loads a weapon model into either the left or right hand slot based on the isLeft flag.
        /// </summary>
        /// <param name="weaponItem">The weapon item to be loaded.</param>
        /// <param name="isLeft">If true, loads the weapon into the left hand slot; otherwise, into the right hand slot.</param>
        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            if (isLeft)
            {
                leftHandSlot.LoadWeaponModel(weaponItem);
                LoadLeftWeaponDamageCollider();
            }
            else
            {
                rightHandSlot.LoadWeaponModel(weaponItem);
                LoadRightWeaponDamageCollider();
            }
        }

        private void LoadLeftWeaponDamageCollider()
        {
            leftHandDamageCollider = leftHandDamageCollider.GetComponentInChildren<DamageCollider>();
        }
        private void LoadRightWeaponDamageCollider()
        {
            rightHandDamageCollider = rightHandDamageCollider.GetComponentInChildren<DamageCollider>();
        }

        public void OpenRightDamageCollider() { rightHandDamageCollider.EnableDamageCollider(); }

        public void OpenLeftDamageCollider() { leftHandDamageCollider.EnableDamageCollider(); }

        public void CloseRightDamageCollider() { rightHandDamageCollider.DisableDamageCollider(); }

        public void CloseLeftDamageCollider() { leftHandDamageCollider.DisableDamageCollider(); }
    }
}

