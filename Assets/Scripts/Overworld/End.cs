using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

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

        /// <summary>Audio played on victory.</summary>
        [SerializeField]
        private AudioClip victoryClip;

        /// <summary>Audio played on defeat.</summary>
        [SerializeField]
        private AudioClip defeatClip;

        private void Start()
        {
            this.rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            this.rootVisual.Query<Button>("Ok").First().clicked += () =>
            {
                // Return back to the main menu when the player beats the game.
                StartCoroutine(Game.State.MainMenu());
            };

            // See if less than 10 hours has passed.
            if (Game.PlayerStats.Time < 600)
            {
                // Hide by default.
                this.rootVisual.visible = false;
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
                this.rootVisual.Query<Label>("Win").First().text = this.translation.GetTable()[
                    hasWon ? "Win" : "Lose"
                ].Value;
                this.rootVisual.Query<Label>("Desc").First().text = hasWon
                    ? this.translation.GetTable()["Victory"].GetLocalizedString(
                        Game.PlayerStats.Exp
                    )
                    : this.translation.GetTable()["Defeat"].Value;

                // Show the dialog.
                this.rootVisual.visible = true;

                // Hide the canvas.
                this.canvas.enabled = false;

                // Remove the save so player won't be able to continue the game.
                Game.PlayerStats.RemoveSave();

                // Play victory/defeat sound.
                AudioSource audioSource = this.GetComponent<AudioSource>();
                // Set the audio clip accordingly.
                audioSource.clip = hasWon ? this.victoryClip : this.defeatClip;
                audioSource.Play();
            }

            this.translation.TableChanged += this.OnTableChanged;
        }

        private void OnDisable()
        {
            this.translation.TableChanged -= this.OnTableChanged;
        }

        /// <summary>Set translations when string table changes.</summary>
        private void OnTableChanged(StringTable table)
        {
            this.rootVisual.Query<Button>("Ok").First().text = table["Ok"].Value;
        }
    }
}
