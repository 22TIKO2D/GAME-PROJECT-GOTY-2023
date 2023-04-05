using System.Collections;
using System.Linq;

namespace Battle.Skill
{
    /// <summary>Skill that hits every enemy.</summary>
    public class AOE : IPlayerSkill
    {
        public IEnumerator Use(Player player, Enemy[] enemies)
        {
            yield return player.Roundtrip(
                () =>
                    // Deal damage to all enemies.
                    enemies
                        .ToList()
                        .ForEach(
                            (enemy) =>
                                // Evenly divide damage between enemies.
                                enemy.InflictDamage(Game.PlayerStats.Damage / (uint)enemies.Length)
                        )
            );
        }
    }
}
