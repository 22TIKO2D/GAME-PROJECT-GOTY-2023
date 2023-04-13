using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;

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

        // <summary>String table used for translations.</summary>
        [SerializeField]
        private LocalizedStringTable translation;

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

            // Set translations.
            this.translation.TableChanged += (table) =>
            {
                rootVisual.Query<Button>("Shutdown").First().text = table["Shutdown"].Value;
                rootVisual.Query<Button>("Apuva").First().text = table["Apuva"].Value;
                rootVisual.Query<Button>("Stats").First().text = table["Stats"].Value;
                rootVisual.Query<Button>("Items").First().text = table["Items"].Value;
                rootVisual.Query<Button>("Settings").First().text = table["Settings"].Value;
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
            closeButton.text = this.translation.GetTable()["Close"].Value;
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
                        // If player has already beaten this NPC.
                        if (Game.PlayerStats.Beaten.Contains(npc.Name))
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
                        string difficulty = this.translation.GetTable()[
                            "Difficulty "
                                + Math.Clamp(
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
                                )
                        ].Value;

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
            this.app.Add(
                new Label(
                    this.translation.GetTable()["Name"].GetLocalizedString(Game.PlayerStats.Name)
                )
            ); // Name
            this.app.Add(
                new Label(
                    this.translation.GetTable()["Exp"].GetLocalizedString(Game.PlayerStats.Exp)
                )
            ); // Experience
            this.app.Add(
                new Label(
                    this.translation.GetTable()["Money"].GetLocalizedString(Game.PlayerStats.Money)
                )
            ); // Money
            this.app.Add(
                new Label(
                    this.translation.GetTable()["Max Health"].GetLocalizedString(
                        Game.PlayerStats.MaxHealth
                    )
                )
            ); // Health
            this.app.Add(
                new Label(
                    this.translation.GetTable()["Damage"].GetLocalizedString(
                        Game.PlayerStats.Damage
                    )
                )
            ); // Damage
            this.app.Add(
                new Label(
                    this.translation.GetTable()["Phone/Speed"].GetLocalizedString(
                        Game.PlayerStats.Speed
                    )
                )
            ); // Speed

            // If player has some skills.
            if (Game.PlayerStats.Skills.Count > 0)
            {
                // Get the names of the skills.
                string[] skillNames = Game.PlayerStats.Skills
                    .Select(
                        (skill) =>
                            this.translation.GetTable()["Skills/" + skill].Value
                            // Skill has been upgraded.
                            + (Game.PlayerStats.SkillUpgrades.Contains(skill) ? "+" : "")
                    )
                    .ToArray();
                this.app.Add(
                    new Label(
                        $"{this.translation.GetTable()["Skills"].Value}\n{string.Join(",\n", skillNames)}"
                    )
                ); // Skills
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
                                this.translation.GetTable()["Item Count"].GetLocalizedString(
                                    item.Name,
                                    itemPair.Value,
                                    item.Health
                                )
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
