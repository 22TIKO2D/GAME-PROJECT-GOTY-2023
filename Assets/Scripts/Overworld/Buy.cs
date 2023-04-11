using UnityEngine;
using UnityEngine.UI;

namespace Overworld
{
    /// <summary>Buy an item button.</summary>
    public class Buy : MonoBehaviour
    {
        /// <summary>Child button.</summary>
        [SerializeField]
        private GameObject buyButton;

        /// <summary>Padding when clamping the button to screen edges.</summary>
        [SerializeField]
        private Vector2 padding;

        /// <summary>Main camera to adjust the button's position to.</summary>
        [SerializeField]
        private Camera mainCamera;

        /// <summary>The items seen in the shop window.</summary>
        private string[] products;

        /// <summary>The target vending machine.</summary>
        private GameObject targetMachine = null;

        /// <summary>The offset at which this talk button is shown at.</summary>
        [SerializeField]
        private Vector2 offset;

        private void Start()
        {
            // Find the dialog box.
            Shop shop = GameObject.Find("Shop").GetComponent<Shop>();

            // Show the shop window when clicked.
            this.buyButton
                .GetComponent<Button>()
                .onClick.AddListener(() => shop.Show(this.products));

            this.Hide();
        }

        /// <summary>Show the buy button.</summary>
        public void Show(GameObject target, string[] products)
        {
            // Target the vending machine.
            this.targetMachine = target;

            // Set info.
            this.products = products;

            // Show the button.
            this.buyButton.SetActive(true);
        }

        /// <summary>Hide the buy button.</summary>
        public void Hide()
        {
            // No target.
            this.targetMachine = null;

            // Hide the button.
            this.buyButton.SetActive(false);
        }

        private void Update()
        {
            if (this.targetMachine != null)
            {
                // Set the button at the target's coordinates.
                Vector2 newPosition =
                    RectTransformUtility.WorldToScreenPoint(
                        this.mainCamera,
                        this.targetMachine.transform.position
                    ) + this.offset;

                // Clamp to the screen size.
                newPosition.x = Mathf.Clamp(
                    newPosition.x,
                    this.padding.x,
                    Screen.width - this.padding.x
                );
                newPosition.y = Mathf.Clamp(
                    newPosition.y,
                    this.padding.y,
                    Screen.height - this.padding.y
                );

                this.transform.position = newPosition;
            }
        }
    }
}
