using UnityEngine;

namespace Overworld
{
    /// <summary>Non-player character.</summary>
    public class NPC : MonoBehaviour
    {
        [SerializeField]
        private string[] battleEnemies;

        /// <summary>Enemies seen in the battle.</summary>
        public string[] Enemies => this.battleEnemies;

        /// <summary>The name of this NPC.</summary>
        public string Name => npcName;

        [SerializeField]
        private string npcName;

        /// <summary>Description for the dialog.</summary>
        [SerializeField]
        private string description;

        /// <summary>Button used to talk to the player.</summary>
        private Talk talkButton;

        private void Start()
        {
            this.talkButton = GameObject.Find("Talk").GetComponent<Talk>();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                // Show the talk button.
                this.talkButton.Show(
                    this.gameObject,
                    this.Name,
                    this.description,
                    this.battleEnemies
                );
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                // Hide the talk button.
                this.talkButton.Hide();
            }
        }
    }
}
