using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Battle
{
    /// <summary>Player actor in the battle.</summary>
    public class Player : Actor
    {
        public override string Name => "Pelaaja";

        /// <summary>UI containing battle stats.</summary>
        [SerializeField]
        private UIDocument battleStats;

        /// <summary>Action that the player can perform.</summary>
        private enum Action
        {
            Attack,
            Skill,
        }

        /// <summary>Current action.</summary>
        private Action? action = null;

        /// <summary>Target enemy.</summary>
        private ushort? target = null;

        /// <summary>Selected skill.</summary>
        private ushort? skill = null;

        /// <summary>Group of the base actions.</summary>
        private GroupBox baseGroup;

        /// <summary>Group of the attack targets.</summary>
        private GroupBox targetGroup;

        /// <summary>Group of the player skills.</summary>
        private GroupBox skillGroup;

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
        }

        /// <summary>Label for action text.</summary>
        private Label actionLabel;

        protected override void Awake()
        {
            // Initialize values.
            this.Speed = Game.PlayerStats.Speed;
            this.MaxHealth = Game.PlayerStats.MaxHealth;

            // Initialize skills.
            this.skills = Game.PlayerStats.SkillClasses.ToArray();

            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            this.actionLabel = this.battleStats.rootVisualElement.Query<Label>("ActionLabel");
            this.actionLabel.visible = false;

            GroupBox actionGroup = this.battleStats.rootVisualElement.Query<GroupBox>("ActionBox");
            actionGroup.Clear();

            this.baseGroup = new GroupBox();
            actionGroup.Add(this.baseGroup);

            Button attackButton = new Button(() => this.action = Action.Attack);
            attackButton.text = "Korjaa";
            this.baseGroup.Add(attackButton);

            Button skillsButton = new Button(() => this.action = Action.Skill);
            skillsButton.text = "Kyky";
            this.baseGroup.Add(skillsButton);

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
                cancelButton.text = "Peruuta";
                this.targetGroup.Add(cancelButton);

                // Create target buttons.
                this.targetButtons = new Button[this.enemies.Length];
                for (int i = 0; i < this.enemies.Length; i++)
                {
                    ushort target = (ushort)(i + 1);
                    this.targetButtons[i] = new Button(() => this.target = target);
                    this.targetButtons[i].text = this.enemies[i].Name;
                    this.targetGroup.Add(this.targetButtons[i]);
                }
            }

            { // Skill group.
                this.skillGroup = new GroupBox();
                actionGroup.Add(this.skillGroup);

                // Create cancel button.
                Button cancelButton = new Button(() => this.skill = 0);
                cancelButton.text = "Peruuta";
                this.skillGroup.Add(cancelButton);

                // Create skill buttons.
                for (int i = 0; i < this.skills.Length; i++)
                {
                    ushort skill = (ushort)(i + 1);
                    Button skillButton = new Button(() => this.skill = skill);
                    skillButton.text = this.skills[i].Name;
                    this.skillGroup.Add(skillButton);
                }
            }

            this.ShowGroup(VisibleGroup.None);
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
                            yield return this.Roundtrip(
                                () =>
                                    // Deal damage to the target enemy.
                                    this.enemies[this.target.Value - 1].InflictDamage(
                                        Game.PlayerStats.Damage
                                    )
                            );

                            // Make sure.
                            this.CheckDead();
                        }

                        this.target = null;
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
