using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{
    /// <summary>Agnostic actor for the battle that can be an enemy or the player.</summary>
    public abstract class Actor : MonoBehaviour
    {
        private uint health;

        /// <summary>Health points for this actor.</summary>
        public uint Health
        {
            get => this.health;
            protected set
            {
                int healthBefore = (int)this.health;
                this.health = value;
                // Invoke health change event with the difference.
                this.HealthChange.Invoke((int)this.health - healthBefore);
            }
        }

        /// <summary>Event called when health changes.</summary>
        public UnityEvent<int> HealthChange { get; set; } = new UnityEvent<int>();

        [SerializeField]
        private uint maxHealth;

        /// <summary>Maximum health points for this actor.</summary>
        public uint MaxHealth
        {
            get => this.maxHealth;
            protected set => this.maxHealth = value;
        }

        [SerializeField]
        private uint speed;

        /// <summary>Speed of this actor in battle.</summary>
        public uint Speed
        {
            get => this.speed;
            protected set => this.speed = value;
        }

        /// <summary>Wether this actor is dead or not.</summary>
        public bool IsDead => this.Health == 0;

        /// <summary>Name of this actor.</summary>
        public abstract string Name { get; }

        /// <summary>Apply damage to the actor's health.</summary>
        public void InflictDamage(uint amount)
        {
            // Prevent health from going to zero.
            this.Health -= (uint)Mathf.Min((int)amount, (int)this.Health);
        }

        /// <summary>Heal the actor by increasing health.</summary>
        public void Heal(uint amount)
        {
            // Prevent health from going over maximum health.
            this.Health = (uint)Mathf.Min((int)(this.Health + amount), (int)this.MaxHealth);
        }

        private void Awake()
        {
            // Start with the maximum health.
            this.health = this.MaxHealth;
        }

        /// <summary>Invoked when this actor's turn comes.</summary>
        public abstract IEnumerator Turn();
    }
}
