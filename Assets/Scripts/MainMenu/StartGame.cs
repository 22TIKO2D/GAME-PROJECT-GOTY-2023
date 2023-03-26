using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class StartGame : MonoBehaviour
    {
        /// <summary>Disable this button if no save is available.</summary>
        [SerializeField]
        private bool disableWhenNoSave;

        private void Start()
        {
            if (this.disableWhenNoSave)
            {
                // Disable if no save available.
                this.GetComponent<Button>().interactable = Game.PlayerStats.SavePresent;
            }
        }

        /// <summary>Continue the existing game.</summary>
        public void ContinueGame()
        {
            // Load the player stats.
            Game.PlayerStats.Load();

            // Start the game.
            StartCoroutine(Game.State.Overworld());
        }

        /// <summary>Start a new game.</summary>
        public void NewGame()
        {
            // Reset the stats to start a fresh game.
            Game.PlayerStats.Reset();

            // Start the game.
            StartCoroutine(Game.State.Overworld());
        }
    }
}
