using MyBox;
using UnityEngine;

[ExecuteAlways]
public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }
    
    #region Anchor blocks
    
    [Foldout("Anchor blocks")] public GameObject GoToBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject MoveBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject TeleportBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject RotateBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject StartRotatingBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject StopRotatingBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject MoveAndRotateBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject SetRotationSpeedBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject SetDirectionBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject SetSpeedBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject SetEaseBlockPrefab;
    [Foldout("Anchor blocks")] public GameObject WaitBlockPrefab;
    
    #endregion
    
    private void OnEnable()
    {
        if (Instance == null) Instance = this;
    }
}