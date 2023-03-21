using UnityEngine;

namespace MainMenu
{
    public class StartGame : MonoBehaviour
    {
        /// <summary>Start the game.</summary>
        public void GameStart()
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
    }
}
