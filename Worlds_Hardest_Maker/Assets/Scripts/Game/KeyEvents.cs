using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Controls key events and manages keyboard shortcuts
///     <para>Attach to game manager</para>
/// </summary>
public class KeyEvents : MonoBehaviour
{
    private AlphaTween menuTween;

    private void Update()
    {
        // toggle playing
        if (LevelSessionManager.Instance.IsEdit && Input.GetKeyDown(KeyCode.Space)) PlayManager.Instance.TogglePlay();

        // close panel if esc pressed
        bool closingPanel = false;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (PanelController panel in PanelManager.Instance.Panels)
            {
                if (!panel.Open || !panel.CloseOnEscape) continue;

                PanelManager.Instance.SetPanelOpen(panel, false);
                closingPanel = true;
            }
        }

        // toggle menu
        if (!closingPanel && !MenuManager.Instance.BlockMenu &&
            (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M)))
        {
            menuTween.SetVisible(!menuTween.IsVisible);
        }

        // teleport player to mouse pos
        if (LevelSessionManager.Instance.IsEdit && EditModeManager.Instance.Playing && Input.GetKeyDown(KeyCode.T))
        {
            GameObject player = PlayerManager.GetPlayer();
            if (player != null)
            {
                player.GetComponent<Rigidbody2D>().position = MouseManager.Instance.MouseWorldPosGrid;
                PlayManager.Instance.Cheated = true;
            }
        }

        // rotate if current edit mode is field and rotatable
        FieldType? fieldType =
            (FieldType?)EnumUtils.TryConvertEnum<EditMode, FieldType>(EditModeManager.Instance.CurrentEditMode);

        if (fieldType != null && ((FieldType)fieldType).IsRotatable() && Input.GetKeyDown(KeyCode.R))
        {
            EditModeManager.Instance.EditRotation = (EditModeManager.Instance.EditRotation - 90) % 360;

            if (SelectionManager.Instance.Selecting) SelectionManager.UpdatePreviewRotation();
        }

#if UNITY_EDITOR
        const KeyCode ctrl = KeyCode.Tab;
#else
            KeyCode ctrl = KeyCode.LeftControl;
#endif

        // keyboard shortcuts with ctrl
        if (Input.GetKey(ctrl) && !EditModeManager.Instance.Playing)
        {
            if (Input.GetKeyDown(KeyCode.S)) SaveSystem.SaveCurrentLevel();

            // paste
            if (!CopyManager.Instance.Pasting && Input.GetKey(KeybindManager.Instance.PasteKey))
                StartCoroutine(CopyManager.Instance.PasteCoroutine());
        }

        // check edit mode toggling if no ctrl and not playing
        if (!Input.GetKey(ctrl) && !EditModeManager.Instance.Playing && Input.anyKeyDown) CheckEditModeKeyEvents();
    }

    /// <returns>list of keyboard shortcuts for edit modes</returns>
    public static Dictionary<KeyCode[], EditMode> GetKeyboardShortcuts()
    {
        Dictionary<KeyCode[], EditMode> keys = new()
        {
            { new[] { KeyCode.D }, EditMode.DeleteField },
            { new[] { KeyCode.W }, EditMode.WallField },
            { new[] { KeyCode.S }, EditMode.StartField },
            { new[] { KeyCode.G }, EditMode.GoalField },
            { new[] { KeyCode.O }, EditMode.OneWayField },
            { new[] { KeyCode.W, KeyCode.A }, EditMode.Water },
            { new[] { KeyCode.I }, EditMode.Ice },
            { new[] { KeyCode.V }, EditMode.Void },
            { new[] { KeyCode.P }, EditMode.Player },
            { new[] { KeyCode.C }, EditMode.Coin },
            { new[] { KeyCode.K }, EditMode.GrayKey },
            { new[] { KeyCode.R, KeyCode.K }, EditMode.RedKey },
            { new[] { KeyCode.G, KeyCode.K }, EditMode.GreenKey },
            { new[] { KeyCode.B, KeyCode.K }, EditMode.BlueKey },
            { new[] { KeyCode.Y, KeyCode.K }, EditMode.YellowKey },
            { new[] { KeyCode.H, KeyCode.C }, EditMode.CheckpointField }
        };
        return keys;
    }

    private static void CheckEditModeKeyEvents()
    {
        // get every user shortcut for switching edit mode
        Dictionary<KeyCode[], EditMode> keyboardShortcuts = GetKeyboardShortcuts();

        // check every event and set edit mode accordingly
        foreach (KeyValuePair<KeyCode[], EditMode> shortcut in keyboardShortcuts)
        {
            bool combinationPressed = true;
            foreach (KeyCode shortcutKey in shortcut.Key)
            {
                if (Input.GetKey(shortcutKey)) continue;

                combinationPressed = false;
                break;
            }

            if (combinationPressed) EditModeManager.Instance.CurrentEditMode = shortcut.Value;
        }
    }

    private void Start() => menuTween = ReferenceManager.Instance.Menu.GetComponent<AlphaTween>();
}