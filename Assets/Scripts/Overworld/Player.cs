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

        /// <summary>The main camera in the scene.</summary>
        [SerializeField]
        private Camera mainCamera;

        /// <summary>Target object.</summary>
        public GameObject Target { private get; set; } = null;

        /// <summary>If we want to save the game.</summary>
        private bool wantSave = false;

        /// <summary>The velocity at which the camera zooms.</summary>
        private float cameraZoomVelocity = 0.0f;

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
            // Zoom the camera out when moving.
            this.mainCamera.orthographicSize = Mathf.SmoothDamp(
                this.mainCamera.orthographicSize,
                2.0f + Mathf.Clamp01(this.rb2d.velocity.magnitude),
                ref this.cameraZoomVelocity,
                0.5f
            );

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

        private void LateUpdate()
        {
            // Fix saving on Android.
            if (this.wantSave)
            {
                this.Save();
                this.wantSave = false;
            }
        }

        /// <summary>Save the game.</summary>
        private void Save()
        {
            // Set the player's position before saving.
            Game.PlayerStats.Position = this.transform.position;

            // Save the game on exit.
            Game.PlayerStats.Save();
        }

        // Save on quit (desktop).
        private void OnApplicationQuit() => this.Save();

        // Save on pause (Android).
        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
                this.wantSave = true;
        }

        // Save on lose focus (Android).
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                this.wantSave = true;
        }
    }
}
