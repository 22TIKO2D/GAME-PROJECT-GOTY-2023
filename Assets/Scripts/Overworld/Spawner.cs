using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    /// <summary>Spawn NPCs on the map.</summary>
    public class Spawner : MonoBehaviour
    {
        private void Awake()
        {
            // Get all the spawn points.
            List<Transform> spawnPoints = GameObject
                .FindGameObjectsWithTag("Spawn")
                .Select((go) => go.transform)
                .ToList();

            // Get all NPCs.
            Resources
                .LoadAll<GameObject>("NPCs")
                .ToList()
                .ForEach(
                    (npc) =>
                    {
                        // Select a random spawn point.
                        int spawnIndex = Random.Range(0, spawnPoints.Count - 1);

                        // Create a new NPC at the spawn point.
                        GameObject.Instantiate(npc, spawnPoints[spawnIndex]);

                        // Remove the spawn point so that it won't be selected again.
                        spawnPoints.RemoveAt(spawnIndex);
                    }
                );
        }
    }
}
