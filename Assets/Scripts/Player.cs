using UnityEngine;

namespace Game
{
    /// <summary>Player script for the overworld map.</summary>
    public class Player : MonoBehaviour
    {
        /// <summary>Joystick used to control the player.</summary>
        [SerializeField]
        private Joystick joystick;

        /// <summary>Movement speed of the player.</summary>
        [SerializeField]
        private float moveSpeed;

        /// <summary>Rigidbody used to control the player movement.</summary>
        private Rigidbody2D rb2d;

        private void Start()
        {
            this.rb2d = this.GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            // Move player to the direction of the joystick.
            this.rb2d.velocity = this.joystick.Direction * this.moveSpeed;
        }
    }
}
