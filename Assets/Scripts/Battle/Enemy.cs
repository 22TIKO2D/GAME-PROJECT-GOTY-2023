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

        /// <summary>Player which to deal damage to.</summary>
        private Actor player;

        private void Start()
        {
            // Get the starting position.
            this.startPosition = this.transform.position;

            // Find the player actor.
            this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Actor>();
        }

        private void Update()
        {
            // Move forward or backward.
            this.offset = Mathf.Clamp(
                this.offset + this.moveDir * Time.deltaTime * moveSpeed,
                0.0f,
                1.0f
            );

            // Based on the starting position.
            this.transform.position =
                startPosition + Vector2.left * Mathf.Lerp(0, moveRange, offset);
        }

        public override IEnumerator Turn()
        {
            // Move forward.
            this.moveDir = 1.0f;
            yield return new WaitForSeconds(0.5f);

            // Hurt player.
            this.player.InflictDamage(this.damage);
            yield return new WaitForSeconds(0.5f);

            // Move backward.
            this.moveDir = -1.0f;
            yield return new WaitForSeconds(0.5f);

            this.moveDir = 0.0f;
        }
    }
}
