using UnityEngine;

namespace Game
{
    /// <summary>Non-player character.</summary>
    public class NPC : MonoBehaviour
    {
        /// <summary>Enemies seen in the battle.</summary>
        [SerializeField]
        private string[] battleEnemies;

        /// <summary>The name of this NPC.</summary>
        public string Name => npcName;

        [SerializeField]
        private string npcName;

        private void OnMouseDown()
        {
            // Start the battle when clicked.
            StartCoroutine(State.Battle(this.battleEnemies));
        }
    }
}
