using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSpawner : MonoBehaviour
{
    [FormerlySerializedAs("spawnPoint")] public Transform SpawnPoint;

    private void Start()
    {
        PlayerManager.InstantiatePlayer(SpawnPoint.position, 3, true);
    }
}