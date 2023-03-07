using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>Static state handling global game state.</summary>
    public static class State
    {
        /// <summary>Enemies seen in battle.</summary>
        public static string[] BattleEnemies { get; private set; }

        /// <summary>Player's position on the map.</summary>
        public static Vector2 PlayerPosition { get; private set; } = Vector2.zero;

        /// <summary>Start a battle with specific enemies.</summary>
        public static IEnumerator Battle(string[] enemies)
        {
            // Store player position.
            PlayerPosition = GameObject.FindWithTag("Player").transform.position;

            Fade fade = GameObject.Find("Fade").GetComponent<Fade>();

            // Disable all event systems.
            GameObject
                .FindObjectsOfType<EventSystem>()
                .ToList()
                .ForEach((es) => es.enabled = false);

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

        public static IEnumerator Overworld()
        {
            Fade fade = GameObject.Find("Fade").GetComponent<Fade>();

            // Start fading in.
            fade.FadeDir = 1.0f;

            yield return new WaitForSeconds(1.0f);

            // Load the scene.
            SceneManager.LoadScene("Overworld");

            // Start fading out.
            fade.FadeDir = -1.0f;

            yield return new WaitForSeconds(1.0f);

            // Stop fading.
            fade.FadeDir = 0.0f;
        }
    }
}
