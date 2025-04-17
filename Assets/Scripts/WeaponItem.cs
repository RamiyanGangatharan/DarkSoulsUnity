using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Represents a weapon item in the game. Inherits from the base <see cref="Item"/> class
    /// and includes additional weapon-specific data such as a model prefab, unarmed flag,
    /// and one-handed attack animations.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/ Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("One Handed Attack Animations")]
        public string OneHanded_Light_Attack_01;
        public string OneHanded_Heavy_Attack_01;
    }
}
