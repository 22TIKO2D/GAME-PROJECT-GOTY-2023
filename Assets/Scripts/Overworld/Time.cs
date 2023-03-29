using UnityEngine;
using TMPro;

namespace Overworld
{
    /// <summary>Text that shows the current time.</summary>
    public class Time : MonoBehaviour
    {
        private void Start()
        {
            // Get the time.
            uint minutes = Game.PlayerStats.Time % 60;
            uint hours = Game.PlayerStats.Time / 60;

            // The game begins at time 9:00.
            this.GetComponent<TMP_Text>().text = $"{9 + hours}:{minutes.ToString("00")}";
        }
    }
}
