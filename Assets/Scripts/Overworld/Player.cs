using UnityEngine;

namespace Overworld
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

        /// <summary>Target arrow.</summary>
        private GameObject targetArrow;

        /// <summary>Renderer for the target arrow.</summary>
        private SpriteRenderer targetArrowRenderer;

        /// <summary>Offset used to fade out the arrow from the target.</summary>
        [SerializeField]
        private float targetDistanceOffset;

        /// <summary>Target object.</summary>
        public GameObject Target { private get; set; } = null;

        private void Start()
        {
            this.rb2d = this.GetComponent<Rigidbody2D>();

            this.targetArrow = this.transform.Find("Target").gameObject;
            this.targetArrowRenderer = this.targetArrow.GetComponentInChildren<SpriteRenderer>();

            // Restore player's position.
            this.transform.position = Game.PlayerStats.Position;
        }

        private void FixedUpdate()
        {
            // Move player to the direction of the joystick.
            this.rb2d.velocity = this.joystick.Direction * this.moveSpeed;
        }

        private void Update()
        {
            if (this.Target != null)
            {
                this.targetArrowRenderer.enabled = true;

                // Make the arrow point to the target.
                this.targetArrow.transform.rotation = Quaternion.Euler(
                    0,
                    0,
                    Mathf.Atan2(
                        -(this.Target.transform.position.x - this.transform.position.x),
                        this.Target.transform.position.y - this.transform.position.y
                    ) * Mathf.Rad2Deg
                );

                // Fade out the arrow if close enough to the target.
                Color arrowColor = this.targetArrowRenderer.color;
                arrowColor.a = Mathf.Clamp01(
                    Vector2.Distance(this.transform.position, this.Target.transform.position)
                        - this.targetDistanceOffset
                );
                this.targetArrowRenderer.color = arrowColor;
            }
            else
            {
                // Hide when no target is selected.
                this.targetArrowRenderer.enabled = false;
            }
        }
    }
}
