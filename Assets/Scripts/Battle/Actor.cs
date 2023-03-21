using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

        /// <summary>Maximum health points for this actor.</summary>
        public uint MaxHealth { get; protected set; }

        /// <summary>Speed of this actor in battle.</summary>
        public uint Speed { get; protected set; }

        /// <summary>Wether this actor is dead or not.</summary>
        public bool IsDead => this.Health == 0;

        /// <summary>Name of this actor.</summary>
        public abstract string Name { get; }

        /// <summary>Movement speed when the enemy takes action.</summary>
        private const float moveSpeed = 4.0f;

        /// <summary>The range of movement.</summary>
        private const uint moveRange = 100;

        /// <summary>Starting position of this enemy.</summary>
        private Vector2 startPosition;

        /// <summary>Current offset when moving.</summary>
        private float offset = 0.0f;

        /// <summary>Target movement direction.</summary>
        private float moveDir = 0.0f;

        private Image image;

        /// <summary>Target color when health changes.</summary>
        private Color targetColor;

        /// <summary>Speed at which color changes.</summary>
        private const float colorChangeSpeed = 6.0f;

        /// <summary>The current amount of color.</summary>
        private float colorAmount = 0.0f;

        /// <summary>Text shown when receiving damage.</summary>
        private GameObject damageText;

        protected virtual void Start()
        {
            this.damageText = Resources.Load<GameObject>("Damage");

            this.image = this.GetComponent<Image>();

            // Listen to the health change events.
            this.HealthChange.AddListener(this.OnHealthChange);

            // Get the starting position.
            this.startPosition = this.transform.position;
        }

        protected virtual void Awake()
        {
            // Start with the maximum health.
            this.health = this.MaxHealth;
        }

        protected virtual void Update()
        {
            // Rapidly reduce the amount of color.
            this.colorAmount -= Time.deltaTime * colorChangeSpeed;
            this.image.color = Color.Lerp(Color.white, this.targetColor, this.colorAmount);

            // Move forward or backward.
            this.offset = Mathf.Clamp(
                this.offset + this.moveDir * Time.deltaTime * moveSpeed,
                0.0f,
                1.0f
            );

            // Based on the starting position.
            this.transform.position =
                startPosition
                + Vector2.left * this.transform.localScale.x * Mathf.Lerp(0, moveRange, offset);
        }

        /// <summary>Apply damage to the actor's health.</summary>
        public void InflictDamage(uint amount)
        {
            uint minAmount = amount - (amount / 2);
            uint maxAmount = amount + (amount / 2);

            // Prevent health from going to zero.
            this.Health -= (uint)Mathf.Min(Random.Range(minAmount, maxAmount), this.Health);
        }

        /// <summary>Heal the actor by increasing health.</summary>
        public void Heal(uint amount)
        {
            uint minAmount = amount - (amount / 2);
            uint maxAmount = amount + (amount / 2);

            // Prevent health from going over maximum health.
            this.Health += (uint)
                Mathf.Min(Random.Range(minAmount, maxAmount), this.MaxHealth - this.Health);
        }

        /// <summary>Move this actor forward.</summary>
        public void MoveForward() => this.moveDir = 1.0f;

        /// <summary>Move this actor backward.</summary>
        public void MoveBackward() => this.moveDir = -1.0f;

        /// <summary>Stop moving this actor.</summary>
        public void StopMoving() => this.moveDir = 0.0f;

        /// <summary>Invoked when this actor's turn comes.</summary>
        public abstract IEnumerator Turn();

        /// <summary>Invoked when actor takes damage.</summary>
        private void OnHealthChange(int amount)
        {
            if (amount < 0)
            {
                // Take damage.
                this.targetColor = Color.red;
                this.colorAmount = 1.0f;
            }
            else if (amount > 0)
            {
                // Heal.
                this.targetColor = Color.green;
                this.colorAmount = 1.0f;
            }

            // Show floating damage number.
            GameObject damageObject = GameObject.Instantiate(this.damageText);

            // Set the amount of damage.
            damageObject.GetComponent<Damage>().Amount = amount;

            // Center the damage text to this actor.
            damageObject.transform.position =
                this.transform.position
                + new Vector3(this.GetComponent<RectTransform>().sizeDelta.x / 2, 0.0f)
                - new Vector3(damageObject.GetComponent<RectTransform>().sizeDelta.x / 2, 0.0f);
        }
    }
}
