using UnityEngine;

namespace Overworld
{
    /// <summary>Non-player character.</summary>
    public class NPC : MonoBehaviour
    {
        /// <summary>Enemies seen in the battle.</summary>
        [SerializeField]
        private string[] battleEnemies;

        /// <summary>The name of this NPC.</summary>
        public string Name => npcName;

        [SerializeField]
        private string npcName;

        /// <summary>Description for the dialog.</summary>
        [SerializeField]
        private string description;

        /// <summary>Dialog UI element.</summary>
        private Dialog dialog;

        private void Start()
        {
            this.dialog = GameObject.Find("Dialog").GetComponent<Dialog>();
        }

        private void OnMouseDown()
        {
            // Show the dialog when clicked.
            this.dialog.Show(this.Name, this.description, this.battleEnemies);
        }
    }
}
