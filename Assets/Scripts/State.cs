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

        /// <summary>Start a battle with specific enemies.</summary>
        public static IEnumerator Battle(
            string[] enemies,
            string afterName,
            string afterDesc,
            string skill
        )
        {
            // Store player position.
            PlayerStats.Position = GameObject.FindWithTag("Player").transform.position;

            // Save before starting the battle.
            PlayerStats.Save();

            // Set after save.
            Game.PlayerStats.AfterDialogName = afterName;
            Game.PlayerStats.AfterDialogDesc = afterDesc;
            Game.PlayerStats.SkillGain = skill;

            // Disable all event systems.
            GameObject
                .FindObjectsOfType<EventSystem>()
                .ToList()
                .ForEach((es) => es.enabled = false);

            // Set the enemies.
            BattleEnemies = enemies;

            yield return ChangeScene("Battle");
        }

        /// <summary>Switch to the overworld scene.</summary>
        public static IEnumerator Overworld() => ChangeScene("Overworld");

        /// <summary>Switch to the main menu scene.</summary>
        public static IEnumerator MainMenu() => ChangeScene("MainMenu");

        /// <summary>Change the scene with a fade effect.</summary>
        private static IEnumerator ChangeScene(string name)
        {
            Fade fade = Fade.Get();

            fade.FadeIn();

            yield return new WaitForSeconds(1.0f);

            // Load the scene.
            SceneManager.LoadScene(name);

            fade.FadeOut();

            yield return new WaitForSeconds(1.0f);
        }
    }
}
