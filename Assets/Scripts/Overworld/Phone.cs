using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    /// <summary>Phone UI.</summary>
    public class Phone : MonoBehaviour
    {
        /// <summary>Buttons group.</summary>
        private GroupBox buttonsGroup;

        /// <summary>NPCs group.</summary>
        private GroupBox npcsGroup;

        /// <summary>Back button in the Apuva! menu.</summary>
        private Button backButton;

        /// <summary>Root phone visual element.</summary>
        private VisualElement phone;

        /// <summary>Button to show the phone screen.</summary>
        private VisualElement phoneButton;

        /// <summary>Player character.</summary>
        private Player player;

        /// <summary>Menu that is currently seen.</summary>
        private enum Menu
        {
            Main,
            Apuva,
        }

        private void Start()
        {
            this.player = GameObject.FindWithTag("Player").GetComponent<Player>();

            VisualElement rootVisual = this.GetComponent<UIDocument>().rootVisualElement;

            this.phone = rootVisual.Query<VisualElement>("Phone");

            this.phoneButton = rootVisual.Query<VisualElement>("PhoneButton");
            // Show the phone screen when the phone button is clicked.
            this.phoneButton.RegisterCallback<ClickEvent>((_) => this.SetVisible(true));

            this.buttonsGroup = rootVisual.Query<GroupBox>("Buttons");
            this.npcsGroup = rootVisual.Query<GroupBox>("NPCs");

            this.SetMenuVisible(Menu.Main);

            // Hide when shutdown.
            rootVisual.Query<Button>("Shutdown").First().clicked += () => this.SetVisible(false);

            // Show Apuva! when this button is clicked.
            rootVisual.Query<Button>("Apuva").First().clicked += this.ShowApuva;

            this.backButton = rootVisual.Query<Button>("Back");

            // Hide Apuva! when back is clicked.
            this.backButton.clicked += () => this.SetMenuVisible(Menu.Main);

            // Hide on start.
            this.SetVisible(false);
        }

        /// <summary>Show the Apuva! app list.</summary>
        private void ShowApuva()
        {
            this.npcsGroup.Clear();
            this.npcsGroup.Add(this.backButton);

            this.SetMenuVisible(Menu.Apuva);

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
                            this.SetMenuVisible(Menu.Main);
                            this.SetVisible(false);
                        });
                        npcButton.text = npc.Name;
                        this.npcsGroup.Add(npcButton);
                    }
                );
        }

        /// <summary>Show or hide phone UI.</summary>
        private void SetVisible(bool isVisible)
        {
            this.phone.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            this.phoneButton.style.display = isVisible ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void SetMenuVisible(Menu menu)
        {
            switch (menu)
            {
                case Menu.Main:
                    this.buttonsGroup.style.display = DisplayStyle.Flex;
                    this.npcsGroup.style.display = DisplayStyle.None;
                    break;

                case Menu.Apuva:
                    this.buttonsGroup.style.display = DisplayStyle.None;
                    this.npcsGroup.style.display = DisplayStyle.Flex;
                    break;
            }
        }
    }
}
