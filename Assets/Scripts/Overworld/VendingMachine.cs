using UnityEngine;

namespace Overworld
{
    /// <summary>A vending machine with items to buy.</summary>
    public class VendingMachine : MonoBehaviour
    {
        /// <summary>The items you can buy at this vending machine.</summary>
        [SerializeField]
        private string[] items;

        /// <summary>Button used to show the shop window.</summary>
        private Buy buyButton;

        private void Start()
        {
            this.buyButton = GameObject.Find("Buy").GetComponent<Buy>();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                // Show the buy button.
                this.buyButton.Show(this.gameObject, this.items);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                // Hide the buy button.
                this.buyButton.Hide();
            }
        }
    }
}
