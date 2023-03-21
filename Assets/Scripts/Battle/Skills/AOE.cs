using System.Collections;
using UnityEngine;

namespace Battle.Skill
{
    public class AOE : IPlayerSkill
    {
        public string Name => "Korjaa kaikkia";

        public IEnumerator Use(Player player, Enemy[] enemies)
        {
            player.MoveForward();
            yield return new WaitForSeconds(0.5f);

            foreach (Enemy enemy in enemies)
            {
                // Deal evenly divided damage to all enemies.
                enemy.InflictDamage(Game.PlayerStats.Damage / (uint)enemies.Length);
            }
            yield return new WaitForSeconds(0.5f);

            player.MoveBackward();
            yield return new WaitForSeconds(0.5f);

            player.StopMoving();
        }
    }
}
