using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// 
    /// </summary>
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;
        public GameObject currentWeaponModel;

        public void UnloadWeapon() { if (currentWeaponModel != null) { currentWeaponModel.SetActive(false); } }
        public void UnloadWeaponAndDestroy() { if (currentWeaponModel != null) { Destroy(currentWeaponModel); } }

        /// <summary>
        /// Instantiates and loads a new weapon model into the weapon slot,
        /// assigning it to the specified transform and resetting its transform values.
        /// Destroys any previously equipped weapon model before loading the new one.
        /// </summary>
        /// <param name="weaponItem">The weapon item to be loaded into the slot. If null, the current weapon is simply unloaded.</param>

        public void LoadWeaponModel(WeaponItem weaponItem)
        {
            UnloadWeaponAndDestroy();

            if (weaponItem == null) { UnloadWeapon(); return; }

            GameObject model = Instantiate(weaponItem.modelPrefab) as GameObject;

            if (model != null)
            {
                model.transform.parent = parentOverride != null ? parentOverride : transform;
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
                currentWeaponModel = model;
            }
        }
    }
}
