using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;

namespace Overworld
{
    /// <summary>Tutorial dialog box shown at the beginning of the game.</summary>
    public class Tutorial : MonoBehaviour
    {
        /// <summary>Enemies seen in the battle.</summary>
        [SerializeField]
        private string[] battleEnemies;

        /// <summary>The main canvas.</summary>
        [SerializeField]
        private Canvas canvas;

        // <summary>String table used for translations.</summary>
        [SerializeField]
        private LocalizedStringTable translation;

        private void Start()
        {
            VisualElement rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            // Show if the player has not seen the tutorial.
            rootVisual.visible = !Game.PlayerStats.SeenTutorial;

            // Hide when player doesn't want to help.
            rootVisual.Query<Button>("No").First().clicked += () =>
            {
                // Hide the dialog.
                rootVisual.visible = false;

                // Show the canvas again.
                this.canvas.enabled = true;

                // Don't show ever again.
                Game.PlayerStats.SeenTutorial = true;
            };

            // Start the battle when player wants to see the tutorial.
            rootVisual.Query<Button>("Yes").First().clicked += () =>
            {
                // Hide the dialog.
                rootVisual.visible = false;

                StartCoroutine(Game.State.Battle(this.battleEnemies, "", "", ""));
            };

            // Set translations.
            this.translation.TableChanged += (table) =>
            {
                rootVisual.Query<Label>("Desc").First().text = table["Intro"].Value;
                rootVisual.Query<Button>("No").First().text = table["No"].Value;
                rootVisual.Query<Button>("Yes").First().text = table["Yes"].Value;
            };
        }
    }
}
