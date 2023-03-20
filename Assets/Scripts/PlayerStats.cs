using UnityEngine;

namespace Game
{
    /// <summary>Stats for the player.</summary>
    public static class PlayerStats
    {
        /// <summary>Good experience collected by the player.</summary>
        public static uint GoodExp { get; set; } = 0;

        /// <summary>Bad experience collected by the player.</summary>
        public static uint BadExp { get; set; } = 0;

        /// <summary>Total experience collected by the player.</summary>
        public static uint TotalExp => GoodExp + BadExp;

        /// <summary>Player's position on the map.</summary>
        public static Vector2 Position { get; set; } = Vector2.zero;

        /// <summary>Maximum health of the player character.</summary>
        public static uint MaxHealth => 100 - BadExp / 2;

        /// <summary>The amount of damage player does to enemies.</summary>
        public static uint Damage => 10 + GoodExp;

        /// <summary>The speed of the player.</summary>
        public static uint Speed => 4 + GoodExp / 20;
    }
}
