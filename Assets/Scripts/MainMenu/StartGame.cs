using UnityEngine;

namespace MainMenu
{
    public class StartGame : MonoBehaviour
    {
        /// <summary>Continue the existing game.</summary>
        public void ContinueGame()
        {
            // Only load if the save is present.
            if (Game.PlayerStats.SavePresent)
            {
                // Load the player stats.
                Game.PlayerStats.Load();
            }

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
