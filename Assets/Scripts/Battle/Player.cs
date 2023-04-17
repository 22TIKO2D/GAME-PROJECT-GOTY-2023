using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;

namespace Battle
{
    /// <summary>Player actor in the battle.</summary>
    public class Player : Actor
    {
        public override string Name => Game.PlayerStats.Name;

        /// <summary>UI containing battle stats.</summary>
        [SerializeField]
        private UIDocument battleStats;

        /// <summary>Action that the player can perform.</summary>
        private enum Action
        {
            Attack,
            Skill,
            Item,
        }

        /// <summary>Current action.</summary>
        private Action? action = null;

        /// <summary>Target enemy.</summary>
        private ushort? target = null;

        /// <summary>Selected skill.</summary>
        private ushort? skill = null;

        /// <summary>Selected item.</summary>
        private string item = null;

        /// <summary>Group of the base actions.</summary>
        private GroupBox baseGroup;

        /// <summary>Group of the attack targets.</summary>
        private GroupBox targetGroup;

        /// <summary>Group of the player skills.</summary>
        private GroupBox skillGroup;

        /// <summary>Group of the player's items.</summary>
        private GroupBox itemGroup;

        /// <summary>Every enemy in this battle.</summary>
        private Enemy[] enemies;

        /// <summary>Target buttons for enemies.</summary>
        private Button[] targetButtons;

        /// <summary>Skills the player can use.</summary>
        private IPlayerSkill[] skills;

        /// <summary>A specific group to show.</summary>
        private enum VisibleGroup
        {
            None,
            Base,
            Target,
            Skill,
            Item,
        }

        /// <summary>Label for action text.</summary>
        private Label actionLabel;

        // <summary>String table used for translations.</summary>
        [SerializeField]
        private LocalizedStringTable translation;

        /// <summary>A state that the tutorial is in.</summary>
        private enum TutorialStage
        {
            None,
            Attack,
            Item,
            Skill,
            Done,
        }

        /// <summary>The current state of the tutorial.</summary>
        private TutorialStage tutorialStage;

        /// <summary>The target choosing attack button.</summary>
        private Button attackButton;

        protected override void Awake()
        {
            // Initialize values.
            this.Speed = Game.PlayerStats.Speed;
            this.MaxHealth = Game.PlayerStats.MaxHealth;

            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            this.actionLabel = this.battleStats.rootVisualElement.Query<Label>("ActionLabel");
            this.actionLabel.visible = false;

            ScrollView actionGroup = this.battleStats.rootVisualElement.Query<ScrollView>(
                "ActionBox"
            );
            actionGroup.Clear();

            this.baseGroup = new GroupBox();
            actionGroup.Add(this.baseGroup);

            this.attackButton = new Button(() => this.action = Action.Attack);
            this.attackButton.text = this.translation.GetTable()["Fix"].Value;
            this.baseGroup.Add(this.attackButton);

            Button skillsButton = new Button(() => this.action = Action.Skill);
            skillsButton.text = this.translation.GetTable()["Player/Skills"].Value;
            this.baseGroup.Add(skillsButton);

            Button itemsButton = new Button(() => this.action = Action.Item);
            itemsButton.text = this.translation.GetTable()["Items"].Value;
            this.baseGroup.Add(itemsButton);

            { // Target group.
                this.targetGroup = new GroupBox();
                actionGroup.Add(this.targetGroup);

                // Find all enemies.
                this.enemies = GameObject
                    .FindGameObjectsWithTag("Enemy")
                    .ToList()
                    .Select(go => go.GetComponent<Enemy>())
                    .ToArray();

                // Create cancel button.
                Button cancelButton = new Button(() => this.target = 0);
                cancelButton.text = this.translation.GetTable()["Cancel"].Value;
                this.targetGroup.Add(cancelButton);

                // Padding to align with the other elements.
                this.targetGroup.Add(new Label(this.translation.GetTable()["Fix"].Value));

                // Create target buttons.
                this.targetButtons = new Button[this.enemies.Length];
                for (int i = 0; i < this.enemies.Length; i++)
                {
                    ushort target = (ushort)(i + 1);
                    this.targetButtons[i] = new Button(() => this.target = target);
                    this.targetButtons[i].text = this.enemies[i].Fix;
                    this.targetGroup.Add(this.targetButtons[i]);
                }
            }

            { // Skill group.
                this.skillGroup = new GroupBox();
                actionGroup.Add(this.skillGroup);

                // Build the group.
                this.BuildSkillGroup();
            }

            { // Item group.
                this.itemGroup = new GroupBox();
                actionGroup.Add(this.itemGroup);

                // Build the group.
                this.BuildItemGroup();
            }

            // Set translations.
            this.actionLabel.text = this.translation.GetTable()["Action"].Value;
            this.battleStats.rootVisualElement.Query<Label>("TurnLabel").First().text =
                this.translation.GetTable()["Turn"].Value;
            this.battleStats.rootVisualElement.Query<Label>("HealthLabel").First().text =
                this.translation.GetTable()["Health"].Value;

            this.HealthChange.AddListener(
                (change) =>
                {
                    // A hacky way to check if the skill was used.
                    // Works because the tutorial skill `Double` also damages the player.
                    if (
                        this.tutorialStage == TutorialStage.Skill
                        && this.skill != null
                        && change < 0
                    )
                        // Advance to the next stage of the tutorial.
                        this.SetTutorial(TutorialStage.Done);
                }
            );

            // Start the tutorial if player has not seen it yet.
            this.SetTutorial(
                Game.PlayerStats.SeenTutorial ? TutorialStage.None : TutorialStage.Attack
            );

            this.ShowGroup(VisibleGroup.None);
        }

        /// <summary>Set the tutorial stage.</summary>
        private void SetTutorial(TutorialStage stage)
        {
            Label tutorialLabel = this.battleStats.rootVisualElement.Query<Label>("Tutorial");

            switch (stage)
            {
                case TutorialStage.None:
                    this.attackButton.SetEnabled(true);
                    tutorialLabel.text = "";
                    tutorialLabel.visible = false;
                    break;

                case TutorialStage.Attack:
                    this.attackButton.SetEnabled(true);
                    tutorialLabel.text = this.translation.GetTable()["Tutorial/Attack"].Value;
                    break;

                case TutorialStage.Item:
                    this.attackButton.SetEnabled(false);
                    tutorialLabel.text = this.translation.GetTable()["Tutorial/Item"].Value;

                    // Add tutorial item.
                    Game.PlayerStats.AddItem("Coffee");

                    // Rebuild the group.
                    this.BuildItemGroup();
                    break;

                case TutorialStage.Skill:
                    this.attackButton.SetEnabled(false);
                    tutorialLabel.text = this.translation.GetTable()["Tutorial/Skill"].Value;

                    // Add tutorial skill.
                    Game.PlayerStats.Skills.Add("Double");

                    // Rebuild the group.
                    this.BuildSkillGroup();
                    break;

                case TutorialStage.Done:
                    this.attackButton.SetEnabled(true);
                    tutorialLabel.text = this.translation.GetTable()["Tutorial/Done"].Value;
                    break;
            }

            this.tutorialStage = stage;
        }

        /// <summary>Build the item group.</summary>
        private void BuildItemGroup()
        {
            this.itemGroup.Clear();

            // Create cancel button.
            Button cancelButton = new Button(() => this.item = "");
            cancelButton.text = this.translation.GetTable()["Cancel"].Value;
            this.itemGroup.Add(cancelButton);

            // Get all the items that the player possesses.
            Game.PlayerStats.Items
                .ToList()
                .ForEach(
                    (itemPair) =>
                    {
                        string itemKey = itemPair.Key;

                        // Get the item. Not very efficient.
                        Game.Item item = Resources
                            .Load<GameObject>("Items/" + itemKey)
                            .GetComponent<Game.Item>();

                        Button itemButton = new Button(() => this.item = itemKey);
                        itemButton.text = this.translation.GetTable()[
                            "Item Count"
                        ].GetLocalizedString(item.Name, itemPair.Value, item.Health);
                        this.itemGroup.Add(itemButton);
                    }
                );
        }

        /// <summary>Build the skill group.</summary>
        private void BuildSkillGroup()
        {
            // Initialize skills.
            this.skills = Game.PlayerStats.Skills
                .Select(
                    (skill) =>
                        (
                            (Battle.IPlayerSkill)
                                Activator.CreateInstance(Type.GetType($"Battle.Skill.{skill}"))
                        )
                )
                .ToArray();

            this.skillGroup.Clear();

            // Create cancel button.
            Button cancelButton = new Button(() => this.skill = 0);
            cancelButton.text = this.translation.GetTable()["Cancel"].Value;
            this.skillGroup.Add(cancelButton);

            // Create skill buttons.
            for (int i = 0; i < this.skills.Length; i++)
            {
                ushort skill = (ushort)(i + 1);
                Button skillButton = new Button(() => this.skill = skill);
                string skillName = this.skills[i].GetType().Name;
                skillButton.text =
                    this.translation.GetTable()["Skills/" + skillName].Value
                    // Skill has been upgraded.
                    + (Game.PlayerStats.SkillUpgrades.Contains(skillName) ? "+" : "");
                this.skillGroup.Add(skillButton);
            }
        }

        /// <summary>Show the specific group.</summary>
        private void ShowGroup(VisibleGroup group)
        {
            // Show label when doing an action.
            this.actionLabel.visible = group != VisibleGroup.None;

            this.baseGroup.style.display =
                group == VisibleGroup.Base ? DisplayStyle.Flex : DisplayStyle.None;

            this.targetGroup.style.display =
                group == VisibleGroup.Target ? DisplayStyle.Flex : DisplayStyle.None;

            this.skillGroup.style.display =
                group == VisibleGroup.Skill ? DisplayStyle.Flex : DisplayStyle.None;

            this.itemGroup.style.display =
                group == VisibleGroup.Item ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>Target an enemy and call an action for the target.</summary>
        public IEnumerator TargetEnemy(Func<int, IEnumerator> action)
        {
            this.ShowGroup(VisibleGroup.Target);

            // Wait until the player has chosen a target.
            yield return new WaitUntil(() => this.target != null);

            // Canceled.
            if (this.target == 0)
            {
                this.action = null;
            }
            else
            {
                yield return action(this.target.Value - 1);

                // Make sure.
                this.CheckDead();
            }

            this.target = null;
        }

        public override IEnumerator Turn()
        {
            while (this.action == null)
            {
                this.ShowGroup(VisibleGroup.Base);

                // Wait until the player has chosen what to do.
                yield return new WaitUntil(() => this.action != null);

                switch (this.action.Value)
                {
                    case Action.Attack:
                        // Target an enemy and deal damage to it.
                        yield return this.TargetEnemy(
                            (target) =>
                                this.Roundtrip(() =>
                                {
                                    // Deal damage to the target enemy.
                                    this.enemies[target].InflictDamage(Game.PlayerStats.Damage);

                                    // Advance to the next stage of the tutorial.
                                    if (this.tutorialStage == TutorialStage.Attack)
                                        this.SetTutorial(TutorialStage.Item);
                                })
                        );
                        break;

                    case Action.Skill:
                        this.ShowGroup(VisibleGroup.Skill);

                        // Wait until the player has chosen a skill.
                        yield return new WaitUntil(() => this.skill != null);

                        // Canceled.
                        if (this.skill == 0)
                        {
                            this.action = null;
                        }
                        else
                        {
                            // Use the skill.
                            yield return this.skills[(uint)this.skill - 1].Use(this, this.enemies);

                            // Make sure.
                            this.CheckDead();
                        }

                        this.skill = null;
                        break;

                    case Action.Item:
                        this.ShowGroup(VisibleGroup.Item);

                        // Wait until the player has chosen an item.
                        yield return new WaitUntil(() => this.item != null);

                        // Canceled.
                        if (this.item == "")
                        {
                            this.action = null;
                        }
                        else
                        {
                            // Get the item.
                            Game.Item item = Resources
                                .Load<GameObject>("Items/" + this.item)
                                .GetComponent<Game.Item>();

                            // Remove the item.
                            Game.PlayerStats.RemoveItem(this.item);

                            // Rebuild the group.
                            this.BuildItemGroup();

                            // Heal the player by the amount this item restores.
                            this.Heal(item.Health);

                            // Advance to the next stage of the tutorial.
                            if (this.tutorialStage == TutorialStage.Item)
                                this.SetTutorial(TutorialStage.Skill);

                            yield return new WaitForSeconds(0.5f);
                        }

                        this.item = null;
                        break;
                }
            }

            // Reset the current action for the next turn.
            this.action = null;

            // Hide actions.
            this.ShowGroup(VisibleGroup.None);
        }

        /// <summary>Check if enemies are dead and disable them if so.</summary>
        private void CheckDead()
        {
            for (int i = 0; i < this.enemies.Length; i++)
            {
                if (this.enemies[i].IsDead)
                {
                    // Hide the target button if the enemy died.
                    this.targetButtons[i].AddToClassList("dead");
                }
            }
        }
    }
}
