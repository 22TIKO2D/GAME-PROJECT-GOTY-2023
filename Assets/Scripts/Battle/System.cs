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
            public Actor actor;

            /// <summary>Time until the actor's turn.</summary>
            public double waitTime;

            /// <summary>Progress bar showing time until the actor's turn.</summary>
            public ProgressBar timeBar;

            /// <summary>Progress bar showing the actor's health.</summary>
            public ProgressBar healthBar;
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

        /// <summary>Background element in the battle end screen.</summary>
        private VisualElement battleEndBg;

        private void Awake()
        {
            this.battleEndBg = this.battleEnd.rootVisualElement.Query<VisualElement>("Bg");

            // Hide the end screen at the beginning.
            this.battleEndBg.visible = false;
            this.battleEndBg.style.opacity = 0;

            // Go back to the map with the back button.
            Button backButton = this.battleEnd.rootVisualElement.Query<Button>("Back");
            backButton.clicked += () => StartCoroutine(Game.State.Overworld());
        }

        private void OnEnable()
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
        }

        private void Start()
        {
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
                data.healthBar.title = $"{data.actor.Health}/{data.actor.MaxHealth}";
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

            bool isPlayerDead = false;
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
                            GroupBox problemGroup =
                                this.battleEnd.rootVisualElement.Query<GroupBox>("Problem");

                            // Choose the text based on if the player lost or won.
                            problemGroup.text = isPlayerDead
                                ? "Ongelma ei ratkennut"
                                : "Ongelma korjattu";
                            problemGroup.AddToClassList(isPlayerDead ? "lost" : "win");

                            // Battle ended.
                            isBattleInProgress = false;
                            break;
                        }
                    }
                }

                // Resume after turns are finished (if battle is still ongoing).
                this.isAdvancing = isBattleInProgress;
            }

            // Count cumulative experience gain.
            uint expGain = (uint)
                this.actors
                    .Where((data) => data.actor.gameObject.tag == "Enemy")
                    .Sum((data) => ((Enemy)data.actor).ExpGain);

            if (isPlayerDead)
                Game.PlayerStats.BadExp += expGain;
            else
                Game.PlayerStats.GoodExp += expGain;

            // Show the experience gain.
            string expType = isPlayerDead ? "huonoa" : "hyv????";
            this.battleEnd.rootVisualElement.Query<Label>("Exp").First().text =
                $"Sait {expGain} {expType} kokemusta.";

            // Hide battle stats.
            this.battleStats.rootVisualElement.visible = false;

            // Show end screen.
            this.battleEndBg.visible = true;
            this.battleEndBg.style.opacity = 1;
        }
    }
}
