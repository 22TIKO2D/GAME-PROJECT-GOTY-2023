using System.Collections;
using UnityEngine;

namespace Battle
{
    /// <summary>Enemy actor in the battle.</summary>
    public class Enemy : Actor
    {
        /// <summary>Name of this enemy.</summary>
        [SerializeField]
        private string enemyName;

        public override string Name => this.enemyName;

        /// <summary>The amount of damage this enemy deals to the player.</summary>
        [SerializeField]
        private uint damage;

        /// <summary>Player which to deal damage to.</summary>
        private Actor player;

        /// <summary>Experience gain from defeating this enemy.</summary>
        [SerializeField]
        private uint expGain;

        /// <summary>Experience gain from defeating this enemy.</summary>
        public uint ExpGain => this.expGain;

        /// <summary>Maximum health points for this enemy.</summary>
        [SerializeField]
        private uint maxHealth;

        /// <summary>Speed of this enemy in battle.</summary>
        [SerializeField]
        private uint speed;

        protected override void Awake()
        {
            // Initialize actor values with the serialized fields.
            this.MaxHealth = this.maxHealth;
            this.Speed = this.speed;

            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            // Find the player actor.
            this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Actor>();
        }

        public override IEnumerator Turn()
        {
            this.MoveForward();
            yield return new WaitForSeconds(0.5f);

            // Hurt the player.
            this.player.InflictDamage(this.damage);
            yield return new WaitForSeconds(0.5f);

            this.MoveBackward();
            yield return new WaitForSeconds(0.5f);

            this.StopMoving();
        }
    }
}
