using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>Fade in or out the screen when switching scenes.</summary>
    public class Fade : MonoBehaviour
    {
        /// <summary>The current amount of fade.</summary>
        private float fadeAmount = 0.0f;

        /// <summary>The direction at which we are fading.</summary>
        public float fadeDir = -1.0f;

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
            this.fadeAmount = Mathf.Clamp01(
                this.fadeAmount + this.fadeDir * this.fadeSpeed * Time.deltaTime
            );

            // Fade white color.
            this.fadeImage.color = Color.Lerp(
                new Color(1, 1, 1, 0), // Transparent
                new Color(1, 1, 1, 1), // Opaque
                this.fadeAmount
            );
        }

        /// <summary>Get or instantiate a fade.</summary>
        public static Fade Get()
        {
            return (
                GameObject.Find("Fade")
                ?? GameObject.Instantiate(Resources.Load<GameObject>("Fade"))
            ).GetComponent<Fade>();
        }

        /// <summary>Fade in by increasing alpha.</summary>
        public void FadeIn() => this.fadeDir = 1.0f;

        /// <summary>Fade out by decreasing alpha.</summary>
        public void FadeOut() => this.fadeDir = -1.0f;
    }
}
