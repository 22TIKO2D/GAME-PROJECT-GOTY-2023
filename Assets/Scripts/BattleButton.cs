using UnityEngine;

namespace Game
{
    public class BattleButton : MonoBehaviour
    {
        public void OnClick()
        {
            // Start a battle.
            StartCoroutine(State.Battle(new string[] { "Enemy2", "Enemy1", "Enemy2" }));
        }
    }
}
