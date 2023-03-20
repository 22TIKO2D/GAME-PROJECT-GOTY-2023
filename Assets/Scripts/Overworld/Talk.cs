using UnityEngine;
using UnityEngine.UI;

namespace Overworld
{
    /// <summary>Talk to the NPC button.</summary>
    public class Talk : MonoBehaviour
    {
        /// <summary>Name of the NPC.</summary>
        private string npcName;

        /// <summary>Description for the NPC.</summary>
        private string npcDesc;

        /// <summary>Enemies in the battle.</summary>
        private string[] npcEnemies;

        /// <summary>Child button.</summary>
        [SerializeField]
        private GameObject talkButton;

        /// <summary>Main camera to adjust the button's position to.</summary>
        [SerializeField]
        private Camera mainCamera;

        /// <summary>The target npc.</summary>
        private GameObject targetNpc = null;

        /// <summary>The offset at which this talk button is shown at.</summary>
        [SerializeField]
        private Vector2 offset;

        private void Start()
        {
            // Find the dialog box.
            Dialog dialog = GameObject.Find("Dialog").GetComponent<Dialog>();

            // Show the dialog when clicked.
            this.talkButton
                .GetComponent<Button>()
                .onClick.AddListener(() => dialog.Show(npcName, npcDesc, npcEnemies));

            this.Hide();
        }

        /// <summary>Show the talk button.</summary>
        public void Show(GameObject target, string name, string desc, string[] enemies)
        {
            // Target the NPC.
            this.targetNpc = target;

            // Set info.
            this.npcName = name;
            this.npcDesc = desc;
            this.npcEnemies = enemies;

            // Show the button.
            this.talkButton.SetActive(true);
        }

        /// <summary>Hide the talk button.</summary>
        public void Hide()
        {
            // No target.
            this.targetNpc = null;

            // Hide the button.
            this.talkButton.SetActive(false);
        }

        private void Update()
        {
            if (this.targetNpc != null)
            {
                // Set the button at the target's coordinates.
                this.transform.position =
                    RectTransformUtility.WorldToScreenPoint(
                        this.mainCamera,
                        this.targetNpc.transform.position
                    ) + this.offset;
            }
        }
    }
}
