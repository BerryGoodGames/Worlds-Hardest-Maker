using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    private void Start()
    {
        PlayerManager.InstantiatePlayer(spawnPoint.position, 3, true);
    }
}
