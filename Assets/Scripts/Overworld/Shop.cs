using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;

namespace Overworld
{
    /// <summary>Shop view.</summary>
    public class Shop : MonoBehaviour
    {
        /// <summary>Shop root visual element.</summary>
        private VisualElement rootVisual;

        /// <summary>The shop view where the items are.</summary>
        private ScrollView shopView;

        /// <summary>The label that shows how much money the player has left.</summary>
        private Label moneyLabel;

        /// <summary>The main canvas.</summary>
        [SerializeField]
        private Canvas canvas;

        // <summary>String table used for translations.</summary>
        [SerializeField]
        private LocalizedStringTable translation;

        private void Start()
        {
            this.rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            // Get the shop view.
            this.shopView = this.rootVisual.Query<ScrollView>("Shop");

            // Get the money label.
            this.moneyLabel = this.rootVisual.Query<Label>("Money");

            // Hide by default.
            this.rootVisual.visible = false;

            // Hide when player doesn't want to help.
            this.rootVisual.Query<Button>("Close").First().clicked += () =>
            {
                // Hide the dialog.
                this.rootVisual.visible = false;

                // Show the canvas again.
                this.canvas.enabled = true;
            };

            // Set translations.
            this.translation.TableChanged += (table) =>
            {
                this.rootVisual.Query<Label>("Buy").First().text = table[
                    // Get from the Buy button.
                    "Overworld/Canvas/Buy/Button/Text (TMP)"
                ].Value;
                this.rootVisual.Query<Button>("Close").First().text = table["Close"].Value;
                this.moneyLabel.text = table["Money"].GetLocalizedString(Game.PlayerStats.Money);
            };
        }

        public void Show(string[] products)
        {
            // Clear the items.
            this.shopView.Clear();

            // Set the money text.
            this.moneyLabel.text = this.translation.GetTable()["Money"].GetLocalizedString(
                Game.PlayerStats.Money
            );

            products
                .ToList()
                .ForEach(
                    (product) =>
                    {
                        // Get the item. Not very efficient.
                        Game.Item item = Resources
                            .Load<GameObject>("Items/" + product)
                            .GetComponent<Game.Item>();

                        Button itemButton = new Button(() =>
                        {
                            // See if player has enough money.
                            if (Game.PlayerStats.Money >= item.Value)
                            {
                                // Give the item to the player.
                                Game.PlayerStats.AddItem(product);

                                // Take the money away.
                                Game.PlayerStats.Money -= item.Value;

                                // Update the text.
                                this.moneyLabel.text = this.translation.GetTable()[
                                    "Money"
                                ].GetLocalizedString(Game.PlayerStats.Money);
                            }
                        });
                        itemButton.text = this.translation.GetTable()["Item"].GetLocalizedString(
                            item.Name,
                            item.Value,
                            item.Health
                        );
                        this.shopView.Add(itemButton);
                    }
                );

            // Show the dialog.
            this.rootVisual.visible = true;

            // Hide the canvas.
            this.canvas.enabled = false;
        }
    }
}
