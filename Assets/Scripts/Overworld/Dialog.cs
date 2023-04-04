using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Overworld
{
    /// <summary>Dialog box shown before battle.</summary>
    public class Dialog : MonoBehaviour
    {
        /// <summary>Dialog root visual element.</summary>
        private VisualElement rootVisual;

        /// <summary>Enemies seen in the battle.</summary>
        private string[] battleEnemies;

        /// <summary>NPC's name in the after dialog.</summary>
        private string afterName;

        /// <summary>NPC's description in the after dialog.</summary>
        private string afterDesc;

        /// <summary>Skill to gain after battle.</summary>
        private string skill;

        /// <summary>The main canvas.</summary>
        [SerializeField]
        private Canvas canvas;

        private void Start()
        {
            this.rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            // Hide by default.
            this.rootVisual.visible = false;

            // Hide when player doesn't want to help.
            this.rootVisual.Query<Button>("DontHelp").First().clicked += () =>
            {
                // Hide the dialog.
                this.rootVisual.visible = false;

                // Show the canvas again.
                this.canvas.enabled = true;
            };

            // Start the battle when player wants to help.
            this.rootVisual.Query<Button>("Help").First().clicked += () =>
            {
                StartCoroutine(
                    Game.State.Battle(
                        this.battleEnemies,
                        this.afterName,
                        this.afterDesc,
                        this.skill
                    )
                );
            };
        }

        public void Show(string name, string desc, string afterDesc, string[] enemies, string skill)
        {
            this.afterName = name;
            this.afterDesc = afterDesc;
            this.skill = skill;

            // Calculate the difficulty.
            ushort difficulty = (ushort)(
                (
                    enemies
                        .ToList()
                        .Select(
                            (enemy) =>
                                // Not very efficient to load the resources every time
                                // we calculate the difficulty, but good enough.
                                Resources
                                    .Load<GameObject>("Enemies/" + enemy)
                                    .GetComponent<Battle.Enemy>()
                        )
                        // Sum the difficulty of the enemies.
                        .Sum((enemy) => enemy.Difficulty)
                    // 5 stars.
                    * 5
                )
                // Divided by the player's power.
                / (Game.PlayerStats.Power * 2)
            );

            // Set the difficulty level.
            this.SetDifficulty(difficulty);

            // Set name and description.
            this.rootVisual.Query<Label>("Name").First().text = name;
            this.rootVisual.Query<Label>("Desc").First().text = desc;

            // Set the enemies for battle.
            this.battleEnemies = enemies;

            // Show the dialog.
            this.rootVisual.visible = true;

            // Hide the canvas.
            this.canvas.enabled = false;
        }

        /// <summary>Set the difficulty level.</summary>
        private void SetDifficulty(ushort level)
        {
            // Get the difficulty stars.
            VisualElement[] stars = this.rootVisual
                .Query<GroupBox>("Difficulty")
                .First()
                .Children()
                .ToArray();

            // Active stars.
            for (int i = 0; i <= Mathf.Min(level, stars.Length - 1); i++)
            {
                stars[i].style.opacity = 1.0f;
            }

            // Dim stars.
            for (int i = level + 1; i < stars.Length; i++)
            {
                stars[i].style.opacity = 0.2f;
            }
        }
    }
}
