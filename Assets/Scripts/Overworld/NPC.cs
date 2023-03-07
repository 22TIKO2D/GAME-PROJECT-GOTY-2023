using UnityEngine;

namespace Game
{
    /// <summary>Non-player character.</summary>
    public class NPC : MonoBehaviour
    {
        /// <summary>Enemies seen in the battle.</summary>
        [SerializeField]
        private string[] battleEnemies;

        private void OnMouseDown()
        {
            // Start the battle when clicked.
            StartCoroutine(State.Battle(this.battleEnemies));
        }
    }
}
