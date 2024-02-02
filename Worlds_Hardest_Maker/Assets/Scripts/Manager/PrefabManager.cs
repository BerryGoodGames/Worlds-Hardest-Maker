using MyBox;
using UnityEngine;

[ExecuteAlways]
public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    #region Entities

    [Foldout("Entities")] public PlayerController Player;

    [Foldout("Entities")] public GameObject Anchor;
    [Foldout("Entities")] public GameObject AnchorBall;

    [Foldout("Entities")] public CoinController Coin;

    [Foldout("Entities")] public KeyController GrayKey;
    [Foldout("Entities")] public KeyController RedKey;
    [Foldout("Entities")] public KeyController GreenKey;
    [Foldout("Entities")] public KeyController BlueKey;
    [Foldout("Entities")] public KeyController YellowKey;

    #endregion

    #region UI

    [Foldout("UI")] public GameObject DropdownOptionPrefab;
    [Foldout("UI")] public GameObject CheckboxOptionPrefab;
    [Foldout("UI")] public GameObject SliderOptionPrefab;
    [Foldout("UI")] public GameObject NumberInputOptionPrefab;
    [Foldout("UI")] public GameObject HeaderOptionPrefab;
    [Foldout("UI")] public GameObject SpaceOptionPrefab;
    [Foldout("UI")] public AlphaTween GlowPrefab;

    [Foldout("UI")] public GameObject FillPreview;
    [Foldout("UI")] public KeyCodeDisplay KeyCodeDisplay;

    #endregion

    #region Anchor blocks

    [Foldout("Anchor blocks")] public AnchorPathLine AnchorPathLine;
    [Foldout("Anchor blocks")] public GameObject AnchorConnector;
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