using UnityEngine;

namespace Game
{
    /// <summary>An item that the player can have.</summary>
    public class Item : MonoBehaviour
    {
        [SerializeField]
        private uint value;

        /// <summary>The price of this item.</summary>
        public uint Value => this.value;

        [SerializeField]
        private uint health;

        /// <summary>The amount of health this item restores.</summary>
        public uint Health => this.health;

        [SerializeField]
        private string itemName;

        /// <summary>The name of this item.</summary>
        public string Name => this.itemName;
    }
}
