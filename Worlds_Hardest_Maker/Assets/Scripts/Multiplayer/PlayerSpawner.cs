using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform SpawnPoint;

    private void Start() => PlayerManager.InstantiatePlayer(SpawnPoint.position, 3, true);
}