using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace MainMenu
{
    /// <summary>New game dialog.</summary>
    public class NewGame : MonoBehaviour
    {
        /// <summary>Root visual element.</summary>
        private VisualElement rootVisual;

        /// <summary>Canvas to hide when this dialog is shown.</summary>
        [SerializeField]
        private Canvas mainCanvas;

        private void Start()
        {
            this.rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            // Hide when canceled.
            this.rootVisual.Query<Button>("Cancel").First().clicked += () => this.SetVisible(false);

            // Start a new game.
            this.rootVisual.Query<Button>("Start").First().clicked += this.StartNewGame;

            // Hide at the start.
            this.SetVisible(false);
        }

        /// <summary>Start a new game.</summary>
        private void StartNewGame()
        {
            TextField nameField = this.rootVisual.Query<TextField>("Name");

            // If name has been given.
            if (nameField.text.Trim() != "")
            {
                // Reset the stats to start a fresh game.
                Game.PlayerStats.Reset();

                // Set the player's name.
                Game.PlayerStats.Name = nameField.text.Trim();

                // Use hash to obfuscate which name to use for god mode.
                using (SHA256 sha256 = SHA256.Create())
                {
                    string hash = string.Join(
                        "",
                        sha256
                            // Compute the sha256 hash sum for the player's name.
                            .ComputeHash(Encoding.UTF8.GetBytes(Game.PlayerStats.Name))
                            // Convert bytes to hex.
                            .Select((b) => b.ToString("x2"))
                    );

                    // God mode.
                    if (hash == "8bd33ca56aad8232f3c2b5969113165222bda18b0be6918cf4201297f116d91f")
                    {
                        // Add some money.
                        Game.PlayerStats.Money = 9999;

                        // Add some experience.
                        Game.PlayerStats.Exp = 9999;

                        // Change the name to indicate a god mode.
                        Game.PlayerStats.Name = "Jumala " + Game.PlayerStats.Name;
                    }
                }

                // Start the game.
                StartCoroutine(Game.State.Overworld());
            }
        }

        /// <summary>Show or hide this new game dialog.</summary>
        public void SetVisible(bool visible)
        {
            // Reset the name field.
            this.rootVisual.Query<TextField>("Name").First().value = "";

            this.rootVisual.visible = visible;
            this.mainCanvas.enabled = !visible;
        }
    }
}
