using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Battle
{
    /// <summary>Player actor in the battle.</summary>
    public class Player : Actor
    {
        public override string Name => "Player";

        /// <summary>Amount of health to restore when player heals.</summary>
        [SerializeField]
        private uint healAmount;

        /// <summary>UI containing battle stats.</summary>
        [SerializeField]
        private UIDocument battleStats;

        /// <summary>Action that the player can perform.</summary>
        private enum Action
        {
            Attack,
            Heal,
        }

        /// <summary>Current action.</summary>
        private Action? action = null;

        /// <summary>Target enemy.</summary>
        private ushort? target = null;

        /// <summary>Group of the base actions.</summary>
        private GroupBox baseGroup;

        /// <summary>Group of the attack targets.</summary>
        private GroupBox targetGroup;

        /// <summary>Every enemy in this battle.</summary>
        private Actor[] enemies;

        /// <summary>Target buttons for enemies.</summary>
        private Button[] targetButtons;

        /// <summary>A specific group to show.</summary>
        private enum VisibleGroup
        {
            None,
            Base,
            Target,
        }

        protected override void Start()
        {
            base.Start();

            GroupBox actionGroup = this.battleStats.rootVisualElement.Query<GroupBox>("ActionBox");
            actionGroup.Clear();

            this.baseGroup = new GroupBox();
            actionGroup.Add(this.baseGroup);

            Button attackButton = new Button(() => this.action = Action.Attack);
            attackButton.text = "Attack";
            this.baseGroup.Add(attackButton);

            Button healButton = new Button(() => this.action = Action.Heal);
            healButton.text = "Heal";
            this.baseGroup.Add(healButton);

            this.targetGroup = new GroupBox();
            actionGroup.Add(this.targetGroup);

            // Find all enemies.
            this.enemies = GameObject
                .FindGameObjectsWithTag("Enemy")
                .ToList()
                .Select(go => go.GetComponent<Actor>())
                .ToArray();

            Button cancelButton = new Button(() => this.target = 0);
            cancelButton.text = "Cancel";
            this.targetGroup.Add(cancelButton);

            this.targetButtons = new Button[this.enemies.Length];
            for (int i = 0; i < this.enemies.Length; i++)
            {
                ushort target = (ushort)(i + 1);
                this.targetButtons[i] = new Button(() => this.target = target);
                this.targetButtons[i].text = this.enemies[i].Name;
                this.targetGroup.Add(this.targetButtons[i]);
            }

            this.ShowGroup(VisibleGroup.None);
        }

        /// <summary>Show the specific group.</summary>
        private void ShowGroup(VisibleGroup group)
        {
            this.baseGroup.style.display =
                group == VisibleGroup.Base ? DisplayStyle.Flex : DisplayStyle.None;

            this.targetGroup.style.display =
                group == VisibleGroup.Target ? DisplayStyle.Flex : DisplayStyle.None;
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
                            this.MoveForward();
                            yield return new WaitForSeconds(0.5f);

                            // Hurt the target enemy.
                            this.enemies[this.target.Value - 1].InflictDamage(75);
                            yield return new WaitForSeconds(0.5f);

                            this.MoveBackward();
                            yield return new WaitForSeconds(0.5f);

                            this.StopMoving();

                            if (this.enemies[this.target.Value - 1].IsDead)
                            {
                                // Hide the target button if the enemy died.
                                this.targetButtons[this.target.Value - 1].AddToClassList("dead");
                            }
                        }

                        this.target = null;
                        break;

                    case Action.Heal:
                        // Heal the player.
                        this.Heal(this.healAmount);

                        yield return new WaitForSeconds(0.5f);
                        break;
                }
            }

            // Reset the current action for the next turn.
            this.action = null;

            // Hide actions.
            this.ShowGroup(VisibleGroup.None);
        }
    }
}
