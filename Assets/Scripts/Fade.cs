using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Fade : MonoBehaviour
    {
        /// <summary>The current amount of fade.</summary>
        private float fadeAmount = 0.0f;

        /// <summary>The direction at which we are fading.</summary>
        public float FadeDir { get; set; } = 0.0f;

        /// <summary>Speed at which we fade.</summary>
        [SerializeField]
        private float fadeSpeed = 1.0f;

        /// <summary>Image used for fading.</summary>
        [SerializeField]
        private RawImage fadeImage;

        private void Start()
        {
            // Keep fade when switching scenes.
            GameObject.DontDestroyOnLoad(this);
        }

        private void Update()
        {
            // Increase or decrease fade.
            this.fadeAmount = Mathf.Clamp(
                this.fadeAmount + this.FadeDir * this.fadeSpeed * Time.deltaTime,
                0.0f,
                1.0f
            );

            // Fade white color.
            this.fadeImage.color = Color.Lerp(
                new Color(1, 1, 1, 0), // Transparent
                new Color(1, 1, 1, 1), // Opaque
                this.fadeAmount
            );
        }
    }
}
