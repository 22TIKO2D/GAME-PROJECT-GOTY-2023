using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace Overworld
{
    /// <summary>Tutorial dialog box shown at the beginning of the game.</summary>
    public class Tutorial : MonoBehaviour
    {
        /// <summary>Tutorial dialog root visual element.</summary>
        private VisualElement rootVisual;

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
            this.rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            // Show if the player has not seen the tutorial.
            this.rootVisual.visible = !Game.PlayerStats.SeenTutorial;

            // Hide when player doesn't want to help.
            this.rootVisual.Query<Button>("No").First().clicked += () =>
            {
                // Hide the dialog.
                this.rootVisual.visible = false;

                // Show the canvas again.
                this.canvas.enabled = true;

                // Don't show ever again.
                Game.PlayerStats.SeenTutorial = true;
            };

            // Start the battle when player wants to see the tutorial.
            this.rootVisual.Query<Button>("Yes").First().clicked += () =>
            {
                // Hide the dialog.
                this.rootVisual.visible = false;

                StartCoroutine(Game.State.Battle(this.battleEnemies, "", "", ""));
            };

            this.translation.TableChanged += this.OnTableChanged;
        }

        private void OnDisable()
        {
            this.translation.TableChanged -= this.OnTableChanged;
        }

        /// <summary>Set translations when string table changes.</summary>
        private void OnTableChanged(StringTable table)
        {
            this.rootVisual.Query<Label>("Name").First().text = table["Golden"].Value;
            this.rootVisual.Query<Label>("Desc").First().text = table["Intro"].Value;
            this.rootVisual.Query<Button>("No").First().text = table["No"].Value;
            this.rootVisual.Query<Button>("Yes").First().text = table["Yes"].Value;
        }
    }
}
