using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    /// <summary>Player actor in the battle.</summary>
    public class Player : Actor
    {
        public override string Name => "Player";

        /// <summary>Amount of health to restore when player heals.</summary>
        [SerializeField]
        private uint healAmount;

        private Image image;

        /// <summary>Target color when health changes.</summary>
        private Color targetColor;

        /// <summary>Speed at which color changes.</summary>
        private const float colorChangeSpeed = 6.0f;

        /// <summary>The current amount of color.</summary>
        private float colorAmount = 0.0f;

        /// <summary>Enemies in the current battle.</summary>
        private List<Actor> enemies;

        public override IEnumerator Turn()
        {
            yield return new WaitForSeconds(0.5f);

            // TODO: Implement player controls.
            this.Heal(this.healAmount);

            // Damage enemies.
            // this.enemies.ForEach(enemy => enemy.InflictDamage(75));

            yield return new WaitForSeconds(0.5f);
        }

        private void Start()
        {
            this.image = this.GetComponent<Image>();

            // Find all enemies.
            this.enemies = GameObject
                .FindGameObjectsWithTag("Enemy")
                .ToList()
                .Select(go => go.GetComponent<Actor>())
                .ToList();

            // Listen to the health change events.
            this.HealthChange.AddListener(this.OnHealthChange);
        }

        private void Update()
        {
            // Rapidly reduce color amount.
            this.colorAmount -= Time.deltaTime * colorChangeSpeed;
            this.image.color = Color.Lerp(Color.white, this.targetColor, this.colorAmount);
        }

        /// <summary>Invoked when player takes damage.</summary>
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
        }
    }
}
