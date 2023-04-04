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

                // Start the game.
                StartCoroutine(Game.State.Overworld());
            }
        }

        /// <summary>Show or hide this new game dialog.</summary>
        public void SetVisible(bool visible)
        {
            this.rootVisual.visible = visible;
            this.mainCanvas.enabled = !visible;
        }
    }
}
