using UnityEngine;
using UnityEngine.Serialization;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance { get; private set; } // singleton

    [FormerlySerializedAs("selectionMouseButton")] [Header("Key binds")]
    public int SelectionMouseButton;

    [FormerlySerializedAs("panMouseButton")] public int PanMouseButton;

    [FormerlySerializedAs("entityDeleteKey")] public KeyCode EntityDeleteKey;

    [FormerlySerializedAs("entityMoveKey")] public KeyCode EntityMoveKey;

    [FormerlySerializedAs("ballCircleRadiusKey")] public KeyCode BallCircleRadiusKey;

    [FormerlySerializedAs("ballCircleAngleKey")] public KeyCode BallCircleAngleKey;

    [FormerlySerializedAs("editSpeedKey")] public KeyCode EditSpeedKey;
    [FormerlySerializedAs("pasteKey")] public KeyCode PasteKey;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}