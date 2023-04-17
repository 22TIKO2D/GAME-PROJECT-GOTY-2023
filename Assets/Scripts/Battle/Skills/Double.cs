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
                        bool upgraded = Game.PlayerStats.SkillUpgrades.Contains("Double");

                        // And deal double damage to it.
                        enemies[target].InflictDamage(
                            Game.PlayerStats.Damage * (uint)(upgraded ? 4 : 2)
                        );

                        // Also deal damage to the player.
                        player.InflictDamage(Game.PlayerStats.MaxHealth / 4);
                    })
            );
        }
    }
}
