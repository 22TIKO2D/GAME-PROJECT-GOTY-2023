using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

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

        /// <summary>Audio mixer used when fading.</summary>
        [SerializeField]
        private AudioMixer audioMixer;

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

            // Fade the volume.
            this.audioMixer.SetFloat("Volume", this.Linear2dB(1 - this.fadeAmount));
        }

        /// <summary>Convert linear audio to decibels.
        private float Linear2dB(float amount)
        {
            amount = Mathf.Clamp01(amount);
            return Mathf.Approximately(amount, 0) ? -80 : (Mathf.Log10(amount) * 20);
        }

        /// <summary>Get or instantiate a fade.</summary>
        public static Fade Get()
        {
            return (
                GameObject.FindObjectOfType<Fade>()?.gameObject
                ?? GameObject.Instantiate(Resources.Load<GameObject>("Fade"))
            ).GetComponent<Fade>();
        }

        /// <summary>Fade in by increasing alpha.</summary>
        public void FadeIn() => this.fadeDir = 1.0f;

        /// <summary>Fade out by decreasing alpha.</summary>
        public void FadeOut() => this.fadeDir = -1.0f;
    }
}
