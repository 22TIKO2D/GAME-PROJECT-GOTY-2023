using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Battle
{
    /// <summary>Battle system.</summary>
    public class System : MonoBehaviour
    {
        /// <summary>Data related to an actor.</summary>
        private class ActorData
        {
            /// <summary>Actor itself.</summary>
            public Actor actor { get; set; }

            /// <summary>Time until the actor's turn.</summary>
            public double waitTime { get; set; }

            /// <summary>Progress bar showing time until the actor's turn.</summary>
            public ProgressBar timeBar { get; set; }

            /// <summary>Progress bar showing the actor's health.</summary>
            public ProgressBar healthBar { get; set; }
        }

        /// <summary>Every actor participating in the battle.</summary>
        private List<ActorData> actors = new List<ActorData>();

        /// <summary>Maximum speed between actors.</summary>
        private double maxSpeed;

        /// <summary>Advance time when it's no-one's turn.</summary>
        private bool isAdvancing = false;

        /// <summary>UI containing battle stats.</summary>
        [SerializeField]
        private UIDocument battleStats;

        /// <summary>UI when the battle ends.</summary>
        [SerializeField]
        private UIDocument battleEnd;

        /// <summary>Speed at which time progresses.</summary>
        [SerializeField]
        private double speed = 1.0;

        /// <summary>Amount of enemies in the battle.</summary>
        private uint enemyCount = 0;

        private void Awake()
        {
            // Hide the end screen at the beginning.
            this.battleEnd.rootVisualElement.visible = false;
            this.battleEnd.rootVisualElement.style.opacity = 0;
        }

        private void Start()
        {
            // Reset actors.
            this.actors.Clear();
            this.enemyCount = 0;

            // Find canvas.
            GameObject canvas = GameObject.Find("Main Canvas");

            // Find the player actor and add it to actors.
            this.actors.Add(
                new ActorData
                {
                    actor = GameObject.FindGameObjectWithTag("Player").GetComponent<Actor>()
                }
            );

            foreach (string enemy in Game.State.BattleEnemies)
            {
                // Instantiate the enemy.
                GameObject enemyObject = GameObject.Instantiate(
                    // Load the resource.
                    Resources.Load<GameObject>("Enemies/" + enemy)
                );

                RectTransform enemyTransform = enemyObject.GetComponent<RectTransform>();

                // Anchor top right.
                enemyTransform.anchorMin = Vector2.one;
                enemyTransform.anchorMax = Vector2.one;

                // Arbitrary good position.
                enemyTransform.position = new Vector2(-200, -175 - this.enemyCount * 325);

                // Set canvas as parent.
                enemyTransform.SetParent(canvas.transform, false);

                // Add enemies.
                this.actors.Add(new ActorData { actor = enemyObject.GetComponent<Actor>() });
                this.enemyCount++;
            }

            // Get the fastest actor.
            this.maxSpeed = this.actors.Max(data => data.actor.Speed);

            // Progress bar groups.
            GroupBox timesGroup = this.battleStats.rootVisualElement.Query<GroupBox>("TurnBox");
            timesGroup.Clear();
            GroupBox healthGroup = this.battleStats.rootVisualElement.Query<GroupBox>("HealthBox");
            healthGroup.Clear();

            // Create progress bars.
            this.actors.ForEach(data =>
            {
                // Create time bar.
                data.timeBar = new ProgressBar();
                data.timeBar.lowValue = 0;
                data.timeBar.highValue = 1;
                data.timeBar.title = data.actor.Name;
                timesGroup.Add(data.timeBar);

                // Create health bar.
                data.healthBar = new ProgressBar();
                data.healthBar.AddToClassList("health-bar");
                data.healthBar.lowValue = 0;
                data.healthBar.highValue = data.actor.MaxHealth;
                healthGroup.Add(data.healthBar);
            });

            // Initialize time bars.
            this.UpdateTimes();

            // Initialize health bars.
            this.UpdateHealths();

            // Update whenever health changes.
            this.actors.ForEach(
                data => data.actor.HealthChange.AddListener((_) => this.UpdateHealths())
            );

            // Start the battle.
            StartCoroutine(Battle());
        }

        /// <summary>Update time bars' values.</summary>
        private void UpdateTimes()
        {
            this.actors.ForEach(
                data => data.timeBar.value = (float)(data.waitTime / this.maxSpeed)
            );
        }

        /// <summary>Update health bars' values.</summary>
        private void UpdateHealths()
        {
            this.actors.ForEach(data =>
            {
                data.healthBar.value = data.actor.Health;
                data.healthBar.title =
                    $"{data.actor.Name} ({data.actor.Health}/{data.actor.MaxHealth})";
            });
        }

        private void Update()
        {
            if (this.isAdvancing)
            {
                this.actors.ForEach(
                    // Increase time by everyone's speed.
                    data => data.waitTime += Time.deltaTime * data.actor.Speed * this.speed
                );

                // Update time progress bars.
                this.UpdateTimes();
            }
        }

        /// <summary>Battle coroutine.</summary>
        private IEnumerator Battle()
        {
            // Wait a while before starting the battle.
            yield return new WaitForSeconds(1.0f);
            this.isAdvancing = true;

            bool isBattleInProgress = true;
            while (isBattleInProgress)
            {
                // Wait until someone has a turn.
                yield return new WaitUntil(
                    () => this.actors.Any(data => data.waitTime >= this.maxSpeed)
                );

                // Pause while turns are taking place.
                this.isAdvancing = false;

                foreach (ActorData data in this.actors)
                {
                    // Select every actor that has a turn right now.
                    if (data.waitTime >= this.maxSpeed)
                    {
                        if (!data.actor.IsDead)
                        {
                            // Use style for current turn.
                            data.timeBar.AddToClassList("current-turn");
                            yield return data.actor.Turn();
                            data.timeBar.RemoveFromClassList("current-turn");
                        }

                        // Reset time.
                        data.waitTime -= this.maxSpeed;

                        // Update time bars.
                        this.UpdateTimes();

                        bool isPlayerDead = false;
                        uint enemiesDead = 0;
                        this.actors.ForEach(data =>
                        {
                            if (data.actor.IsDead)
                            {
                                switch (data.actor.gameObject.tag)
                                {
                                    case "Player":
                                        // Player seems to be dead.
                                        isPlayerDead = true;
                                        break;
                                    case "Enemy":
                                        // Some enemy died.
                                        enemiesDead++;
                                        break;
                                }

                                // Hide dead actors.
                                data.actor.gameObject
                                    .GetComponent<UnityEngine.UI.Image>()
                                    .enabled = false;
                                data.timeBar.AddToClassList("dead");
                                data.healthBar.AddToClassList("dead");
                            }
                        });

                        // End battle if the player dies or all enemies are dead.
                        if (isPlayerDead || enemiesDead >= this.enemyCount)
                        {
                            Label endLabel = this.battleEnd.rootVisualElement.Query<Label>();
                            // Choose the text based on if the player lost or won.
                            endLabel.text = isPlayerDead ? "YOU DIED" : "VICTORY";

                            // Battle ended.
                            isBattleInProgress = false;
                            break;
                        }
                    }
                }

                // Resume after turns are finished (if battle is still ongoing).
                this.isAdvancing = isBattleInProgress;
            }

            this.EndBattle();
        }

        /// <summary>Called when the battle ends.</summary>
        private void EndBattle()
        {
            // Hide battle stats.
            this.battleStats.rootVisualElement.visible = false;

            // Show end screen.
            this.battleEnd.rootVisualElement.visible = true;
            this.battleEnd.rootVisualElement.style.opacity = 1;
        }
    }
}
