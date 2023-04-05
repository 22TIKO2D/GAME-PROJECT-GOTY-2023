using System.Collections;
using UnityEngine;

namespace Battle.Skill
{
    /// <summary>Skill that heals the player.</summary>
    public class Heal : IPlayerSkill
    {
        public IEnumerator Use(Player player, Enemy[] enemies)
        {
            // Heal the player by 20%.
            player.Heal(Game.PlayerStats.MaxHealth / 5);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
