using UnityEngine;
using UnityEngine.UIElements;

namespace Overworld
{
    /// <summary>Dialog box shown before battle.</summary>
    public class Dialog : MonoBehaviour
    {
        /// <summary>Dialog root visual element.</summary>
        private VisualElement rootVisual;

        /// <summary>Enemies seen in the battle.</summary>
        private string[] battleEnemies;

        private void Start()
        {
            this.rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            // Hide by default.
            this.rootVisual.visible = false;

            // Hide when player doesn't want to help.
            this.rootVisual.Query<Button>("DontHelp").First().clicked += () =>
                this.rootVisual.visible = false;

            // Start the battle when player wants to help.
            this.rootVisual.Query<Button>("Help").First().clicked += () =>
                StartCoroutine(Game.State.Battle(this.battleEnemies));
        }

        public void Show(string name, string desc, string[] enemies)
        {
            // Set name and description.
            this.rootVisual.Query<Label>("Name").First().text = name;
            this.rootVisual.Query<Label>("Desc").First().text = desc;

            // Set the enemies for battle.
            this.battleEnemies = enemies;

            // Show the dialog.
            this.rootVisual.visible = true;
        }
    }
}
