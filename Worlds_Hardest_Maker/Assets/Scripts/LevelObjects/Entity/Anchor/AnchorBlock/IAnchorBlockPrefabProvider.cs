using UnityEngine;

public interface IAnchorBlockPrefabProvider
{
    GameObject GoToBlockPrefab { get; }
    GameObject MoveBlockPrefab { get; }
    GameObject TeleportBlockPrefab { get; }
    GameObject RotateBlockPrefab { get; }
    GameObject StartRotatingBlockPrefab { get; }
    GameObject StopRotatingBlockPrefab { get; }
    GameObject MoveAndRotateBlockPrefab { get; }
    GameObject SetRotationSpeedBlockPrefab { get; }
    GameObject SetDirectionBlockPrefab { get; }
    GameObject SetSpeedBlockPrefab { get; }
    GameObject SetEaseBlockPrefab { get; }
    GameObject WaitBlockPrefab { get; }
}