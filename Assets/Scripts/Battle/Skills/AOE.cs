using System.Collections;
using System.Linq;

namespace Battle.Skill
{
    public class AOE : IPlayerSkill
    {
        public string Name => "Korjaa kaikkia";

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
