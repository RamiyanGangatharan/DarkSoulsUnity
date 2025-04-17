using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Represents a base item in the game. This ScriptableObject contains
    /// general item properties such as the item's icon and name.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "DarkSouls/Item")]
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        public Sprite itemIcon;
        public string itemName;
    }
}
