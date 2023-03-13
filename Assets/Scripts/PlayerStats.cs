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
    }
}
