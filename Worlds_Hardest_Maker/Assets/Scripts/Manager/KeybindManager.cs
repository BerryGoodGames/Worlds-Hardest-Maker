using UnityEngine;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance { get; private set; } // singleton

    [Header("Key binds")] public int SelectionMouseButton;
    public int PanMouseButton;
    public KeyCode EntityDeleteKey;
    public KeyCode EntityMoveKey;
    public KeyCode BallCircleRadiusKey;
    public KeyCode BallCircleAngleKey;
    public KeyCode EditSpeedKey;
    public KeyCode PasteKey;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}