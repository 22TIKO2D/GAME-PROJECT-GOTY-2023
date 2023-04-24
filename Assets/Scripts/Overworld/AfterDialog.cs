using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace Overworld
{
    /// <summary>Dialog box shown after battle.</summary>
    public class AfterDialog : MonoBehaviour
    {
        /// <summary>Dialog root visual element.</summary>
        private VisualElement rootVisual;

        /// <summary>The main canvas.</summary>
        [SerializeField]
        private Canvas canvas;

        // <summary>String table used for translations.</summary>
        [SerializeField]
        private LocalizedStringTable translation;

        private void Start()
        {
            this.rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            this.rootVisual.Query<Button>("Ok").First().clicked += () =>
            {
                // Hide the dialog.
                this.rootVisual.visible = false;

                // Show the canvas again.
                this.canvas.enabled = true;
            };

            // Check if after dialog is set.
            if (Game.PlayerStats.AfterDialogName == "" || Game.PlayerStats.AfterDialogDesc == "")
            {
                // Hide by default.
                this.rootVisual.visible = false;
            }
            else
            {
                // Set the name and description.
                this.rootVisual.Query<Label>("Name").First().text =
                    Game.PlayerStats.AfterDialogName;
                this.rootVisual.Query<Label>("Desc").First().text =
                    Game.PlayerStats.AfterDialogDesc;

                // Reset the after dialog.
                Game.PlayerStats.AfterDialogName = "";
                Game.PlayerStats.AfterDialogDesc = "";

                // Show the dialog.
                this.rootVisual.visible = true;

                // Hide the canvas.
                this.canvas.enabled = false;
            }

            this.translation.TableChanged += this.OnTableChanged;
        }

        private void OnDisable()
        {
            this.translation.TableChanged -= this.OnTableChanged;
        }

        /// <summary>Set translations when string table changes.</summary>
        private void OnTableChanged(StringTable table)
        {
            this.rootVisual.Query<Button>("Ok").First().text = table["Ok"].Value;
        }
    }
}
