using UnityEngine;

namespace DarkSouls
{
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        public Sprite itemIcon;
        public string itemName;
    }
}
