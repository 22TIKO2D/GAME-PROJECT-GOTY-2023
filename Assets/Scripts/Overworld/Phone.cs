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

        private void Start()
        {
            this.player = GameObject.FindWithTag("Player").GetComponent<Player>();

            VisualElement rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            this.phone = rootVisual.Query<VisualElement>("Phone");

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
                        // Create button for this NPC.
                        Button npcButton = new Button(() =>
                        {
                            this.player.Target = npc.gameObject;
                            this.SetVisible(false);
                        });
                        npcButton.text = npc.Name;
                        this.app.Add(npcButton);
                    }
                );
        }

        /// <summary>Show the player stats page.</summary>
        private void ShowStats()
        {
            this.ResetApp();

            // Create experience info labels.
            Label goodExpLabel = new Label(
                $"Sinulla on {Game.PlayerStats.GoodExp} hyvää kokemusta."
            );
            Label badExpLabel = new Label(
                $"Sinulla on {Game.PlayerStats.BadExp} huonoa kokemusta."
            );

            // Set colors.
            goodExpLabel.style.color = Color.green;
            badExpLabel.style.color = Color.red;

            // Add experience info.
            this.app.Add(goodExpLabel);
            this.app.Add(badExpLabel);

            // Add other labels.
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
                string[] skillNames = Game.PlayerStats.SkillClasses
                    .Select((skill) => skill.Name)
                    .ToArray();
                this.app.Add(new Label($"Sinun kykysi: {string.Join(", ", skillNames)}")); // Skills
            }
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
