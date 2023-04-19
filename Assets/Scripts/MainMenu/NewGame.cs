using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;

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

        // <summary>String table used for translations.</summary>
        [SerializeField]
        private LocalizedStringTable translation;

        private void Start()
        {
            this.rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            // Hide when canceled.
            this.rootVisual.Query<Button>("Cancel").First().clicked += () => this.SetVisible(false);

            // Start a new game.
            this.rootVisual.Query<Button>("Start").First().clicked += this.StartNewGame;

            // Set translations.
            this.translation.TableChanged += (table) =>
            {
                this.rootVisual.Query<TextField>("Name").First().label = table["Give Name"].Value;
                this.rootVisual.Query<Button>("Cancel").First().text = table["Cancel"].Value;
                this.rootVisual.Query<Button>("Start").First().text = table["Start"].Value;
                this.rootVisual.Query<GroupBox>("NewGame").First().text = table[
                    // Get from the NewGame button.
                    "MainMenu/Canvas/NewGame/Text (TMP)"
                ].Value;
            };

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
                    if (hash == "d21277cab5cbb0c09bb36c634a0f3478a7c50e9cbdc612ca2e38cf22c452d229")
                    {
                        // Add some money.
                        Game.PlayerStats.Money = 99999;

                        // Add some experience.
                        Game.PlayerStats.Exp = 99999;

                        // Change the name to indicate a god mode.
                        Game.PlayerStats.Name = "<" + Game.PlayerStats.Name + ">";

                        // God doesn't need tutorials.
                        Game.PlayerStats.SeenTutorial = true;
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
