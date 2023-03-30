using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace Game
{
    /// <summary>Settings dialog.</summary>
    public class Settings : MonoBehaviour
    {
        /// <summary>Root visual element.</summary>
        private VisualElement rootVisual;

        /// <summary>Canvas to hide when this dialog is shown.</summary>
        [SerializeField]
        private Canvas mainCanvas;

        /// <summary>Mixer for audio.</summary>
        [SerializeField]
        AudioMixer mixer;

        private void Start()
        {
            this.rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            // Get the music slider.
            Slider musicSlider = this.rootVisual.Query<Slider>("Music");

            // Chagne music volume when slider chages.
            musicSlider.RegisterValueChangedCallback(
                (value) =>
                {
                    // Set volume.
                    this.mixer.SetFloat("MusicVolume", this.Linear2dB(value.newValue));

                    // Save to player prefs.
                    PlayerPrefs.SetFloat("MusicVolume", value.newValue);
                }
            );

            // Set the volume.
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);

            // Hide when closed.
            this.rootVisual.Query<Button>("Close").First().clicked += () =>
            {
                this.SetVisible(false);

                // Save settings when closed.
                PlayerPrefs.Save();
            };

            // Hide at the start.
            this.SetVisible(false);
        }

        /// <summary>Convert linear audio to decibels.
        private float Linear2dB(float amount)
        {
            amount = Mathf.Clamp01(amount);
            return Mathf.Approximately(amount, 0) ? -80 : (Mathf.Log10(amount) * 20);
        }

        /// <summary>Show or hide this settings dialog.</summary>
        public void SetVisible(bool visible)
        {
            this.rootVisual.visible = visible;
            this.mainCanvas.enabled = !visible;
        }
    }
}
