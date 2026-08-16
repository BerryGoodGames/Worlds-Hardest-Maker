using System;
using MyBox;
using UnityEngine;

[Serializable]
public class AnchorBlockPrefabProvider : IAnchorBlockPrefabProvider
{
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject goToBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject moveBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject teleportBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject rotateBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject startRotatingBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject stopRotatingBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject moveAndRotateBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject setRotationSpeedBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject setDirectionBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject setSpeedBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject setEaseBlockPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject waitBlockPrefab;

    public GameObject GoToBlockPrefab => goToBlockPrefab;
    public GameObject MoveBlockPrefab => moveBlockPrefab;
    public GameObject TeleportBlockPrefab => teleportBlockPrefab;
    public GameObject RotateBlockPrefab => rotateBlockPrefab;
    public GameObject StartRotatingBlockPrefab => startRotatingBlockPrefab;
    public GameObject StopRotatingBlockPrefab => stopRotatingBlockPrefab;
    public GameObject MoveAndRotateBlockPrefab => moveAndRotateBlockPrefab;
    public GameObject SetRotationSpeedBlockPrefab => setRotationSpeedBlockPrefab;
    public GameObject SetDirectionBlockPrefab => setDirectionBlockPrefab;
    public GameObject SetSpeedBlockPrefab => setSpeedBlockPrefab;
    public GameObject SetEaseBlockPrefab => setEaseBlockPrefab;
    public GameObject WaitBlockPrefab => waitBlockPrefab;
}