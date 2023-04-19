using UnityEngine;
using UnityEngine.Localization;

namespace Overworld
{
    /// <summary>Non-player character.</summary>
    public class NPC : MonoBehaviour
    {
        /// <summary>Dialog seen when encountering random NPCs for a first few times.</summary>
        private static readonly LocalizedString[] staticDialog =
        {
            new LocalizedString("StringTable", "Random Dialog 1"),
            new LocalizedString("StringTable", "Random Dialog 2"),
            new LocalizedString("StringTable", "Random Dialog 3"),
            new LocalizedString("StringTable", "Random Dialog 4"),
        };

        /// <summary>Dialog seen when encountering random NPCs after all static dialogs are seen.</summary>
        private static readonly LocalizedString[] randomDialog =
        {
            new LocalizedString("StringTable", "Random Dialog After 1"),
            new LocalizedString("StringTable", "Random Dialog After 2"),
            new LocalizedString("StringTable", "Random Dialog After 3"),
            new LocalizedString("StringTable", "Random Dialog After 4"),
            new LocalizedString("StringTable", "Random Dialog After 5"),
            new LocalizedString("StringTable", "Random Dialog After 6"),
            new LocalizedString("StringTable", "Random Dialog After 7"),
            new LocalizedString("StringTable", "Random Dialog After 8"),
        };

        [SerializeField]
        private string[] battleEnemies;

        /// <summary>Enemies seen in the battle.</summary>
        public string[] Enemies => this.battleEnemies;

        /// <summary>The name of this NPC.</summary>
        public string Name => this.npcName;

        [SerializeField]
        private string npcName;

        /// <summary>Description for the dialog.</summary>
        [SerializeField]
        private LocalizedString description;

        /// <summary>Description for the after dialog.</summary>
        [SerializeField]
        private LocalizedString afterDescription;

        /// <summary>Button used to talk to the player.</summary>
        private Talk talkButton;

        [SerializeField]
        private string skill;

        /// <summary>Skill to gain from this NPC.</summary>
        public string Skill => this.skill;

        private void Start()
        {
            this.talkButton = GameObject.Find("Talk").GetComponent<Talk>();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            // If player and player doesn't have the skill this NPC gives.
            if (
                collider.gameObject.tag == "Player"
                // Not beaten yet.
                && !Game.PlayerStats.Beaten.Contains(this.Name)
            )
            {
                // Show the talk button.
                this.talkButton.Show(
                    this.gameObject,
                    this.Name,
                    this.description,
                    // Show static/random dialog when this is a random NPC (no skill).
                    this.skill == ""
                        ? ( // If we still have static dialogues left.
                            Game.PlayerStats.Dialog < staticDialog.Length
                                // Get the static dialog.
                                ? staticDialog[Game.PlayerStats.Dialog]
                                : (
                                    !this.afterDescription.IsEmpty
                                        // Use after dialog if available.
                                        ? this.afterDescription
                                        // Choose a random dialog otherwise.
                                        : randomDialog[Random.Range(0, randomDialog.Length)]
                                )
                        )
                        : this.afterDescription,
                    this.battleEnemies,
                    this.skill
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
