using MyBox;
using UnityEngine;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance { get; private set; } // singleton

    [Header("Key binds")] [DefinedValues(0, 1, 2)] public int SelectionMouseButton;

    [DefinedValues(0, 1, 2)] public int PanMouseButton;
    [SearchableEnum] public KeyCode EntityDeleteKey;
    [SearchableEnum] public KeyCode EntityMoveKey;

    public KeyCode EditSpeedKey;

    [SearchableEnum] public KeyCode PasteKey;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}