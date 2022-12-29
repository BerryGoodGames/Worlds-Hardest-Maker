using UnityEngine;
using UnityEngine.Serialization;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance { get; private set; } // singleton

    [FormerlySerializedAs("SelectionMouseButton")] [Header("Key binds")] public int selectionMouseButton;
    [FormerlySerializedAs("PanMouseButton")] public int panMouseButton;
    [FormerlySerializedAs("EntityDeleteKey")] public KeyCode entityDeleteKey;
    [FormerlySerializedAs("EntityMoveKey")] public KeyCode entityMoveKey;
    [FormerlySerializedAs("BallCircleRadiusKey")] public KeyCode ballCircleRadiusKey;
    [FormerlySerializedAs("BallCircleAngleKey")] public KeyCode ballCircleAngleKey;
    [FormerlySerializedAs("EditSpeedKey")] public KeyCode editSpeedKey;
    [FormerlySerializedAs("PasteKey")] public KeyCode pasteKey;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}