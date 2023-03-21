using System.Collections;

namespace Battle
{
    /// <summary>Skill that the player can have.</summary>
    public interface IPlayerSkill
    {
        /// <summary>Name of the skill.</summary>
        public string Name { get; }

        /// <summary>Experience needed to unlock this skill.</summary>
        public uint Exp { get; }

        /// <summary>Method to use the skill.</summary>
        public IEnumerator Use(Player player, Enemy[] enemies);
    }
}