using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Overworld
{
    /// <summary>Phone UI.</summary>
    public class Phone : MonoBehaviour
    {
        /// <summary>Buttons group.</summary>
        private GroupBox buttons;

        /// <summary>App group.</summary>
        private ScrollView app;

        /// <summary>Root phone visual element.</summary>
        private VisualElement phone;

        /// <summary>Button to show the phone screen.</summary>
        private VisualElement phoneButton;

        /// <summary>Player character.</summary>
        private Player player;

        /// <summary>The main canvas.</summary>
        [SerializeField]
        private Canvas canvas;

        /// <summary>Settings dialog.</summary>
        [SerializeField]
        private Game.Settings settings;

        private void Start()
        {
            this.player = GameObject.FindWithTag("Player").GetComponent<Player>();

            VisualElement rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            this.phone = rootVisual.Query<VisualElement>("Phone");
            // Hide when clicked the empty area.
            this.phone.RegisterCallback<ClickEvent>(
                (e) =>
                {
                    // When clicked this element specifically.
                    if (e.target == e.currentTarget)
                        this.SetVisible(false);
                }
            );

            this.phoneButton = rootVisual.Query<VisualElement>("PhoneButton");
            // Show the phone screen when the phone button is clicked.
            this.phoneButton.RegisterCallback<ClickEvent>((_) => this.SetVisible(true));

            this.buttons = rootVisual.Query<GroupBox>("Buttons");
            this.app = rootVisual.Query<ScrollView>("App");

            // Hide when shutdown.
            rootVisual.Query<Button>("Shutdown").First().clicked += () => this.SetVisible(false);

            // Show Apuva! when this button is clicked.
            rootVisual.Query<Button>("Apuva").First().clicked += this.ShowApuva;

            // Show infos when this button is clicked.
            rootVisual.Query<Button>("Stats").First().clicked += this.ShowStats;

            // Show items when this button is clicked.
            rootVisual.Query<Button>("Items").First().clicked += this.ShowItems;

            // Show the settings dialog when settings button is clicked.
            rootVisual.Query<Button>("Settings").First().clicked += () =>
            {
                // Hide the phone.
                this.SetVisible(false);

                // And show the settings.
                this.settings.SetVisible(true);
            };

            // Hide on start.
            this.SetVisible(false);
        }

        /// <summary>Reset the app view.</summary>
        private void ResetApp()
        {
            this.app.Clear();

            // Add a close/back button.
            Button closeButton = new Button(() => this.ShowApps(true));
            closeButton.text = "Sulje";
            this.app.Add(closeButton);

            // Show the app view.
            this.ShowApps(false);
        }

        /// <summary>Show the Apuva! app list.</summary>
        private void ShowApuva()
        {
            this.ResetApp();

            GameObject
                .FindGameObjectsWithTag("NPC")
                .Select(go => go.GetComponent<NPC>())
                .ToList()
                .ForEach(
                    (npc) =>
                    {
                        // If we have already beaten this enemy.
                        if (Game.PlayerStats.Skills.Contains(npc.Skill))
                            return;

                        // Create button for this NPC.
                        Button npcButton = new Button(() =>
                        {
                            this.player.Target = npc.gameObject;
                            this.SetVisible(false);
                        });

                        // This is a static NPC if it has a skill.
                        if (npc.Skill != "")
                            npcButton.AddToClassList("apuva");

                        // Get the difficulty.
                        string difficulty = Math.Clamp(
                            (
                                (
                                    npc.Enemies
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
                                    // 5 ratings.
                                    * 5
                                )
                                // Divided by the player's power.
                                / (Game.PlayerStats.Power * 2)
                            ),
                            0,
                            4
                        ) switch
                        {
                            0 => "Helppo",
                            1 => "Sopiva",
                            2 => "Kohtuullinen",
                            3 => "Vaikea",
                            4 => "Mahdoton",
                            _ => "???",
                        };

                        npcButton.text = $"{npc.Name} ({difficulty})";

                        this.app.Add(npcButton);
                    }
                );
        }

        /// <summary>Show the player stats page.</summary>
        private void ShowStats()
        {
            this.ResetApp();

            // Add info labels.
            this.app.Add(new Label($"Sinulla on {Game.PlayerStats.Exp} kokemusta."));
            this.app.Add(new Label($"Sinulla on {Game.PlayerStats.Money}€ rahaa.")); // Money
            this.app.Add(new Label($"Sinulla on {Game.PlayerStats.MaxHealth} motivaatiota.")); // Health
            this.app.Add(
                new Label(
                    $"Sinä korjaat ongelmia keskimäärin {Game.PlayerStats.Damage} pisteen tehokkuudella."
                )
            ); // Damage
            this.app.Add(new Label($"Sinun nopeutesi on {Game.PlayerStats.Speed}.")); // Speed

            // If player has some skills.
            if (Game.PlayerStats.Skills.Count > 0)
            {
                // Get the names of the skills.
                string[] skillNames = Game.PlayerStats.Skills
                    .Select(
                        (skill) =>
                            (
                                (Battle.IPlayerSkill)
                                    Activator.CreateInstance(Type.GetType($"Battle.Skill.{skill}"))
                            ).Name
                    )
                    .ToArray();
                this.app.Add(new Label($"Sinun taitosi: {string.Join(", ", skillNames)}")); // Skills
            }
        }

        /// <summary>Show the player's items.</summary>
        private void ShowItems()
        {
            this.ResetApp();

            Game.PlayerStats.Items
                .ToList()
                .ForEach(
                    (itemPair) =>
                    {
                        // Get the item. Not very efficient.
                        Game.Item item = Resources
                            .Load<GameObject>("Items/" + itemPair.Key)
                            .GetComponent<Game.Item>();

                        // Show the item name, its count, and how much health it restores.
                        this.app.Add(
                            new Label(
                                $"{item.Name} (x{itemPair.Value})\npalauttaa {item.Health} motia"
                            )
                        );
                    }
                );
        }

        /// <summary>Show or hide phone UI.</summary>
        private void SetVisible(bool isVisible)
        {
            // Show the apps when shown/hidden.
            this.ShowApps(true);

            this.phone.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            this.phoneButton.style.display = isVisible ? DisplayStyle.None : DisplayStyle.Flex;

            // Disable the canvas the phone is shown.
            this.canvas.enabled = !isVisible;
        }

        /// <summary>Show the main apps panel.</summary>
        private void ShowApps(bool isVisible)
        {
            this.buttons.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            this.app.style.display = isVisible ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}
