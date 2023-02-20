using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Game
{
    /// <summary>Virtual joystick controller UI element.</summary>
    public class Joystick : MonoBehaviour
    {
        /// <summary>Top element moved away relative to the starting position.</summary>
        [SerializeField]
        private RectTransform top;

        /// <summary>Maximum distance the top element can move.</summary>
        [SerializeField]
        private float topMaxDistance;

        /// <summary>Starting position when the finger is pressed down.</summary>
        private Vector2 startPosition = Vector2.zero;

        /// <summary>Touching position when the finger is moved across the screen.</summary>
        private Vector2 touchPosition = Vector2.zero;

        /// <summary>Wether finger is touching the screen or not.</summary>
        private bool isTouching = false;

        /// <summary>Wether touching started over a UI element.</summary>
        private FirstTouch firstTouch = FirstTouch.None;

        /// <summary>State that the first touch was in.</summary>
        private enum FirstTouch
        {
            /// <summary>No touch yet.</summary>
            None,

            /// <summary>Touch was not over any UI element</summary>
            Clean,

            /// <summary>Touch was over some UI element</summary>
            Dirty,
        }

        /// <summary>
        /// Speed and direction at which the joystick is moved.
        /// Values never go beyond [-1.0, 1.0].
        /// </summary>
        public Vector2 direction { get; private set; } = Vector2.zero;

        /// <summary>Finger used for touch inputs.</summary>
        private const int primaryFinger = 0; // First finger to touch the screen.

        private void Awake()
        {
            EnhancedTouchSupport.Enable();

            // When the finger is down, initialize starting position and start touching.
            Touch.onFingerDown += finger =>
            {
                if (finger.index == primaryFinger)
                {
                    this.startPosition = finger.screenPosition;
                    this.touchPosition = this.startPosition;
                    this.isTouching = true;
                }
            };

            // When finger is moved along the screen, set touching position.
            Touch.onFingerMove += finger =>
            {
                if (finger.index == primaryFinger)
                {
                    this.touchPosition = finger.screenPosition;
                }
            };

            // When the finger is released, stop touching.
            Touch.onFingerUp += finger =>
            {
                if (finger.index == primaryFinger)
                {
                    this.isTouching = false;
                }
            };
        }

        private void Update()
        {
            // When touching show the joystick and calculate the direction.
            if (this.isTouching)
            {
                if (this.firstTouch == FirstTouch.None)
                {
                    // Ignore touches that are over some UI element.
                    this.firstTouch = EventSystem.current.IsPointerOverGameObject()
                        ? FirstTouch.Dirty
                        : FirstTouch.Clean;
                }

                // If first touch was not over any UI element.
                if (this.firstTouch == FirstTouch.Clean)
                {
                    this.transform.position = this.startPosition;

                    Vector2 distance = Vector2.ClampMagnitude(
                        this.touchPosition - this.startPosition,
                        // Clamp to the maximum distance.
                        this.topMaxDistance
                    );

                    // Divide with `topMaxDistance` to ensure values between [-1.0, 1.0].
                    this.direction = distance / this.topMaxDistance;

                    // Move away relative to the starting position.
                    this.top.localPosition = distance;
                }

                // If touch is not clean, don't do anything.
            }
            else
            {
                // Move the joystick off-screen.
                this.transform.position = new Vector2(-1000.0f, 0);
                this.top.localPosition = Vector2.zero;

                // Reset the direction.
                this.direction = Vector2.zero;

                // Not touching anymore.
                this.firstTouch = FirstTouch.None;
            }
        }
    }
}
