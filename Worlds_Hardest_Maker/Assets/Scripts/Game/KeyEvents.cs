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
        { "EditMode_Delete", EditMode.Delete },
        { "EditMode_Wall", EditMode.Wall },
        { "EditMode_Start", EditMode.Start },
        { "EditMode_Goal", EditMode.Goal },
        { "EditMode_OneWayGate", EditMode.OneWay },
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
        { "EditMode_Checkpoint", EditMode.Checkpoint },
    };

    private void Update()
    {
        if (CheckKeyBindAddition()) return;

        // pick object
        if (KeyBinds.GetKeyBindDown("Editor_Pick"))
        {
            print("sex2");
            PickManager.PickObject(MouseManager.Instance.MouseWorldPos);
        }
        
        // toggle playing
        if (LevelSessionManager.Instance.IsEdit && KeyBinds.GetKeyBindDown("Editor_PlayLevel")) PlayManager.Instance.TogglePlay();

        bool closingPanel = CheckClosingPanel();

        // toggle menu
        if (!closingPanel && !MenuManager.Instance.BlockMenu &&
            (Input.GetKeyDown(KeyCode.Escape) || KeyBinds.GetKeyBindDown("Editor_Menu")))
            ReferenceManager.Instance.MenuTween.SetVisible(!ReferenceManager.Instance.MenuTween.IsVisible);

        CheckTeleportPlayer();

        CheckEditModeRotation();

        CheckEditorKeyBinds();

        // check edit mode toggling if no ctrl and not playing
        if (!KeyBinds.GetKeyBind("Editor_Modify") && !EditModeManager.Instance.Playing && Input.anyKeyDown) CheckEditModeKeyEvents();
    }

    private void CheckEditorKeyBinds()
    {
        // keyboard shortcuts with ctrl
        if (EditModeManager.Instance.Playing) return;
        
        if (KeyBinds.GetKeyBindDown("Editor_SaveLevel")) SaveSystem.SaveCurrentLevel();

        // paste
        if (!CopyManager.Instance.Pasting && KeyBinds.GetKeyBind("Editor_Paste")) StartCoroutine(CopyManager.Instance.PasteCoroutine());
    }

    private static void CheckEditModeRotation()
    {
        // rotate if current edit mode is field and rotatable
        FieldType? fieldType = (FieldType?)EditModeManager.Instance.CurrentEditMode.TryConvertTo<EditMode, FieldType>();

        if (fieldType == null 
            || !((FieldType)fieldType).GetFieldObject().IsRotatable 
            || !KeyBinds.GetKeyBindDown("Editor_Rotate")) return;
        
        EditModeManager.Instance.EditRotation = (EditModeManager.Instance.EditRotation - 90) % 360;

        if (SelectionManager.Instance.Selecting) SelectionManager.UpdatePreviewRotation();
    }

    private static void CheckTeleportPlayer()
    {
        // teleport player to mouse pos
        if (!LevelSessionManager.Instance.IsEdit || !EditModeManager.Instance.Playing || !KeyBinds.GetKeyBindDown("Editor_TeleportPlayer")) return;
        
        PlayerController player = PlayerManager.GetPlayer();
        if (player == null) return;
        
        player.Rb.position = MouseManager.Instance.MouseWorldPosGrid;
        PlayManager.Instance.Cheated = true;
    }

    private static bool CheckClosingPanel()
    {
        // close panel if esc pressed
        bool closingPanel = false;
        
        if (!Input.GetKeyDown(KeyCode.Escape)) return false;
        
        foreach (PanelController panel in PanelManager.Instance.Panels)
        {
            if (!panel.Open || !panel.CloseOnEscape) continue;

            PanelManager.Instance.SetPanelOpen(panel, false);
            closingPanel = true;
        }

        return closingPanel;
    }

    private bool CheckKeyBindAddition()
    {
        // check if user adding key bind
        if (!MenuManager.Instance.IsAddingKeyBind) return false;
        
        // cancel adding key bind
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            KeyBindSetterController.CancelAddingKeyBind();
            return true;
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

        return true;

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