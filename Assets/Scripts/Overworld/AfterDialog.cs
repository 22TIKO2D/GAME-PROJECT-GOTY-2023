using UnityEngine;
using UnityEngine.UIElements;

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

            // Check if after dialog is set.
            if (
                Game.PlayerStats.AfterDialogName == null || Game.PlayerStats.AfterDialogDesc == null
            )
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
                Game.PlayerStats.AfterDialogName = null;
                Game.PlayerStats.AfterDialogDesc = null;

                // Show the dialog.
                rootVisual.visible = true;

                // Hide the canvas.
                this.canvas.enabled = false;
            }
        }
    }
}
