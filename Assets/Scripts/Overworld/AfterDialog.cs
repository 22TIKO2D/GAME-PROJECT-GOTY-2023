using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;

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
            VisualElement rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            rootVisual.Query<Button>("Ok").First().clicked += () =>
            {
                // Hide the dialog.
                rootVisual.visible = false;

                // Show the canvas again.
                this.canvas.enabled = true;
            };

            // Set translations.
            this.translation.TableChanged += (table) =>
            {
                rootVisual.Query<Button>("Ok").First().text = table["Ok"].Value;
            };

            // Check if after dialog is set.
            if (Game.PlayerStats.AfterDialogName == "" || Game.PlayerStats.AfterDialogDesc == "")
            {
                // Hide by default.
                rootVisual.visible = false;
            }
            else
            {
                // Set the name and description.
                rootVisual.Query<Label>("Name").First().text = Game.PlayerStats.AfterDialogName;
                rootVisual.Query<Label>("Desc").First().text = Game.PlayerStats.AfterDialogDesc;

                // Reset the after dialog.
                Game.PlayerStats.AfterDialogName = "";
                Game.PlayerStats.AfterDialogDesc = "";

                // Show the dialog.
                rootVisual.visible = true;

                // Hide the canvas.
                this.canvas.enabled = false;
            }
        }
    }
}
