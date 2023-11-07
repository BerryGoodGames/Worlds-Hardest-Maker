using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Controls key events and manages keyboard shortcuts
///     <para>Attach to game manager</para>
/// </summary>
public class KeyEvents : MonoBehaviour
{
    private KeyCode[] prevHeldDownKeys = Array.Empty<KeyCode>();

    private static readonly Dictionary<string, EditMode> keyboardShortcuts = new()
    {
        { "EditMode_Delete", EditMode.DeleteField },
        { "EditMode_Wall", EditMode.WallField },
        { "EditMode_Start", EditMode.StartField },
        { "EditMode_Goal", EditMode.GoalField },
        { "EditMode_OneWayGate", EditMode.OneWayField },
        { "EditMode_Water", EditMode.Water },
        { "EditMode_Ice", EditMode.Ice },
        { "EditMode_Void", EditMode.Void },
        { "EditMode_Player", EditMode.Player },
        { "EditMode_Coin", EditMode.Coin },
        { "EditMode_GrayKey", EditMode.GrayKey },
        { "EditMode_RedKey", EditMode.RedKey },
        { "EditMode_GreenKey", EditMode.GreenKey },
        { "EditMode_BlueKey", EditMode.BlueKey },
        { "EditMode_YellowKey", EditMode.YellowKey },
        { "EditMode_Checkpoint", EditMode.CheckpointField },
    };

    private void Update()
    {
        // check if user adding key bind
        if (MenuManager.Instance.IsAddingKeyBind)
        {
            // cancel adding key bind
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                KeyBindSetterController.CancelAddingKeyBind();
                return;
            }

            // else add the key the user typed
            KeyCode[] keysDown = GetKeysDown();
            if (prevHeldDownKeys.Length > keysDown.Length && prevHeldDownKeys.Length > 0)
            {
                MenuManager.Instance.AddingKeyBindSetter.AddKeyCode(prevHeldDownKeys);
                prevHeldDownKeys = Array.Empty<KeyCode>();
                KeyBindSetterController.CancelAddingKeyBind();
            }
            else prevHeldDownKeys = keysDown;

            return;
        }

        // toggle playing
        if (LevelSessionManager.Instance.IsEdit && KeyBinds.GetKeyBindDown("Editor_PlayLevel")) PlayManager.Instance.TogglePlay();

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
            (Input.GetKeyDown(KeyCode.Escape) || KeyBinds.GetKeyBindDown("Editor_Menu"))) ReferenceManager.Instance.MenuTween.SetVisible(!ReferenceManager.Instance.MenuTween.IsVisible);

        // teleport player to mouse pos
        if (LevelSessionManager.Instance.IsEdit && EditModeManager.Instance.Playing && KeyBinds.GetKeyBindDown("Editor_TeleportPlayer"))
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

        if (fieldType != null && ((FieldType)fieldType).IsRotatable() && KeyBinds.GetKeyBindDown("Editor_Rotate"))
        {
            EditModeManager.Instance.EditRotation = (EditModeManager.Instance.EditRotation - 90) % 360;

            if (SelectionManager.Instance.Selecting) SelectionManager.UpdatePreviewRotation();
        }

        // keyboard shortcuts with ctrl
        if (!EditModeManager.Instance.Playing)
        {
            if (KeyBinds.GetKeyBindDown("Editor_SaveLevel")) SaveSystem.SaveCurrentLevel();

            // paste
            if (!CopyManager.Instance.Pasting && KeyBinds.GetKeyBind("Editor_Paste"))
                StartCoroutine(CopyManager.Instance.PasteCoroutine());
        }

        // check edit mode toggling if no ctrl and not playing
        if (!KeyBinds.GetKeyBind("Editor_Modify") && !EditModeManager.Instance.Playing && Input.anyKeyDown) CheckEditModeKeyEvents();
    }

    private static KeyCode[] GetKeysDown()
    {
        List<KeyCode> keysDown = new();

        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(keyCode)) keysDown.Add(keyCode);
        }

        return keysDown.ToArray();
    }

    private static void CheckEditModeKeyEvents()
    {
        // check every event and set edit mode accordingly
        foreach (KeyValuePair<string, EditMode> shortcut in keyboardShortcuts)
        {
            if (KeyBinds.GetKeyBindDown(shortcut.Key)) EditModeManager.Instance.CurrentEditMode = shortcut.Value;
        }
    }
}