using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    private void Start()
    {
        PlayerManager.InstantiatePlayer(spawnPoint.position, 3, true);
    }
}