using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>Static state handling global game state.</summary>
    public static class State
    {
        /// <summary>Enemies seen in battle.</summary>
        public static string[] BattleEnemies { get; private set; }

        /// <summary>Start a battle with specific enemies.</summary>
        public static IEnumerator Battle(string[] enemies)
        {
            Fade fade = GameObject.Find("Fade").GetComponent<Fade>();

            // Start fading in.
            fade.FadeDir = 1.0f;

            yield return new WaitForSeconds(1.0f);

            // Set the enemies.
            BattleEnemies = enemies;

            // Load the scene.
            SceneManager.LoadScene("Battle");

            // Start fading out.
            fade.FadeDir = -1.0f;

            yield return new WaitForSeconds(1.0f);

            // Stop fading.
            fade.FadeDir = 0.0f;
        }
    }
}
