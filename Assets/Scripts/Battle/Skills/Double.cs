using System.Collections;

namespace Battle.Skill
{
    /// <summary>Skill that does double damage, but also damages the player.</summary>
    public class Double : IPlayerSkill
    {
        public IEnumerator Use(Player player, Enemy[] enemies)
        {
            // Target an enemy.
            return player.TargetEnemy(
                (target) =>
                    player.Roundtrip(() =>
                    {
                        uint multiplier = (uint)(
                            // Upgraded?
                            Game.PlayerStats.SkillUpgrades.Contains("Double")
                                ? 4 // Quadruple damage / quarter health.
                                : 2 // Double damage / half health.
                        );

                        // And deal double damage to it.
                        enemies[target].InflictDamage(Game.PlayerStats.Damage * multiplier);

                        // Also deal damage to the player.
                        player.InflictDamage(Game.PlayerStats.Damage / multiplier);
                    })
            );
        }
    }
}
