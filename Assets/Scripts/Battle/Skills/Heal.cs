using System.Collections;
using UnityEngine;

namespace Battle.Skill
{
    /// <summary>Skill that heals the player.</summary>
    public class Heal : IPlayerSkill
    {
        public string Name => "Innostu";

        public uint Exp => 20;

        public IEnumerator Use(Player player, Enemy[] enemies)
        {
            // Heal the player.
            player.Heal(Game.PlayerStats.HealAmount);

            yield return new WaitForSeconds(0.5f);
        }
    }
}