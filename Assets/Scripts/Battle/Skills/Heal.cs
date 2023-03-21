using System.Collections;
using UnityEngine;

namespace Battle.Skill
{
    public class Heal : IPlayerSkill
    {
        public string Name => "Innostu";

        public IEnumerator Use(Player player, Enemy[] enemies)
        {
            // Heal the player.
            player.Heal(Game.PlayerStats.HealAmount);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
