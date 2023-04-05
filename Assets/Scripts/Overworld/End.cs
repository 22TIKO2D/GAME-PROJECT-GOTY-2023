using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;

namespace Overworld
{
    /// <summary>End of the game.</summary>
    public class End : MonoBehaviour
    {
        /// <summary>Dialog root visual element.</summary>
        private VisualElement rootVisual;

        /// <summary>The main canvas.</summary>
        [SerializeField]
        private Canvas canvas;

        // <summary>String table used for translations.</summary>
        [SerializeField]
        private LocalizedStringTable translation;

        private void Start()
        {
            VisualElement rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            rootVisual.Query<Button>("Ok").First().clicked += () =>
            {
                // Return back to the main menu when the player beats the game.
                StartCoroutine(Game.State.MainMenu());
            };

            // Set translations.
            this.translation.TableChanged += (table) =>
            {
                rootVisual.Query<Button>("Ok").First().text = table["Ok"].Value;
            };

            // See if less than 10 hours has passed.
            if (Game.PlayerStats.Time < 600)
            {
                // Hide by default.
                rootVisual.visible = false;
            }
            else
            {
                // If we have collected all skills, we won.
                bool hasWon =
                    Assembly
                        .GetExecutingAssembly()
                        .GetTypes()
                        .Where(
                            (type) =>
                                // In this namespace.
                                type.Namespace == "Battle.Skill"
                                // And inherited from this class.
                                && typeof(Battle.IPlayerSkill).IsAssignableFrom(type)
                        )
                        .Count() == Game.PlayerStats.Skills.Count();

                // Set the win text and description.
                rootVisual.Query<Label>("Win").First().text = this.translation.GetTable()[
                    hasWon ? "Win" : "Lose"
                ].Value;
                rootVisual.Query<Label>("Desc").First().text = hasWon
                    ? this.translation.GetTable()["Victory"].GetLocalizedString(
                        Game.PlayerStats.Exp
                    )
                    : this.translation.GetTable()["Defeat"].Value;

                // Show the dialog.
                rootVisual.visible = true;

                // Hide the canvas.
                this.canvas.enabled = false;

                // Remove the save so player won't be able to continue the game.
                Game.PlayerStats.RemoveSave();
            }
        }
    }
}
