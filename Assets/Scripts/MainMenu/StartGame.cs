using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    /// <summary>Start game buttons.</summary>
    public class StartGame : MonoBehaviour
    {
        private void Start()
        {
            // Disable if no save available.
            this.GetComponent<Button>().interactable = Game.PlayerStats.SavePresent;
        }

        /// <summary>Continue the existing game.</summary>
        public void ContinueGame()
        {
            // Load the player stats.
            Game.PlayerStats.Load();

            // Start the game.
            StartCoroutine(Game.State.Overworld());
        }
    }
}
