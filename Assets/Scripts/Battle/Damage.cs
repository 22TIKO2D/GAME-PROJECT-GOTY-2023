using UnityEngine;
using TMPro;

namespace Battle
{
    /// <summary>Damage shown over actors.</summary>
    public class Damage : MonoBehaviour
    {
        /// <summary>The amount of damage.</summary>
        public int Amount
        {
            set
            {
                // Set the text.
                this.textMesh.text = (value > 0 ? "+" : "") + value.ToString();

                // Set the color.
                if (value > 0)
                    this.textMesh.color = Color.green;
                else if (value < 0)
                    this.textMesh.color = Color.red;
            }
        }

        /// <summary>The speed at which the text scrolls up.</summary>
        [SerializeField]
        private float scrollSpeed;

        /// <summary>The speed at which the text fades out.</summary>
        [SerializeField]
        private float fadeSpeed;

        private TMP_Text textMesh;

        private void Awake()
        {
            this.textMesh = this.GetComponent<TMP_Text>();
        }

        private void Start()
        {
            // Set canvas as the parent.
            this.transform.SetParent(GameObject.Find("Main Canvas").transform, true);
        }

        private void Update()
        {
            // Move up.
            this.transform.position += Vector3.up * Time.deltaTime * this.scrollSpeed;

            // Fade out.
            this.textMesh.color -= new Color(0.0f, 0.0f, 0.0f, Time.deltaTime * this.fadeSpeed);

            // Delete when fully transparent.
            if (this.textMesh.color.a <= 0.0f)
            {
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}
