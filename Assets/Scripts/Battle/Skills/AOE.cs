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
                        // Only alive enemies.
                        .Where((enemy) => !enemy.IsDead)
                        .ToList()
                        .ForEach(
                            (enemy) =>
                            {
                                // Upgraded?
                                if (Game.PlayerStats.SkillUpgrades.Contains("AOE"))
                                    // Do full damage to all enemies.
                                    enemy.InflictDamage(Game.PlayerStats.Damage);
                                else
                                    // Evenly divide damage between enemies.
                                    enemy.InflictDamage(
                                        Game.PlayerStats.Damage / (uint)enemies.Length
                                    );
                            }
                        )
            );
        }
    }
}
