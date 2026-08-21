using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(
    fileName = "AnchorBlockTestConfig",
    menuName = "ScriptableObjects/Test Configs/Anchor Block Test Config"
)]
public sealed class AnchorBlockTestConfig : ScriptableObject
{
    [Tooltip("Name of the real game/editor scene that boots all runtime managers and DI.")] [Scene]
    public string applicationSceneName;

    [Tooltip("An empty position in the level where the test anchor can be created.")]
    public Vector2 anchorSpawnPosition = new(9999f, 9999f);

    [Tooltip("Maximum time allowed for the playtest loop smoke test.")]
    [Min(0.1f)]
    public float loopTimeoutSeconds = 2f;
}