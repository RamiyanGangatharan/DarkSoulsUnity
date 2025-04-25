using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Represents a weapon item in the game. Inherits from the base <see cref="Item"/> class
    /// and includes additional weapon-specific data such as a model prefab and an unarmed flag.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/ Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("One Handed Attack Animations")]
        public string OH_Light_Attack_1;
        public string OH_Heavy_Attack_1;
    }
}
