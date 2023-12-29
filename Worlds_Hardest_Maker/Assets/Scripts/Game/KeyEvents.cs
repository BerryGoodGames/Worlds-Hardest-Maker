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

    // private static readonly Dictionary<string, EditMode> keyboardShortcuts = new()
    // {
    //     { "EditMode_Delete", EditModeManager.Delete },
    //     { "EditMode_Wall", EditModeManager.Wall },
    //     { "EditMode_Start", EditModeManager.Start },
    //     { "EditMode_Goal", EditModeManager.Goal },
    //     { "EditMode_OneWayGate", EditModeManager.OneWay },
    //     { "EditMode_Water", EditModeManager.Water },
    //     { "EditMode_Ice", EditModeManager.Ice },
    //     { "EditMode_Void", EditModeManager.Void },
    //     { "EditMode_Player", EditModeManager.Player },
    //     { "EditMode_Coin", EditModeManager.Coin },
    //     { "EditMode_GrayKey", EditModeManager.GrayKey },
    //     { "EditMode_RedKey", EditModeManager.RedKey },
    //     { "EditMode_GreenKey", EditModeManager.GreenKey },
    //     { "EditMode_BlueKey", EditModeManager.BlueKey },
    //     { "EditMode_YellowKey", EditModeManager.YellowKey },
    //     { "EditMode_Checkpoint", EditModeManager.Checkpoint },
    // };

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
        if (!KeyBinds.GetKeyBind("Editor_Modify") && !EditModeManagerOther.Instance.Playing && Input.anyKeyDown) CheckEditModeKeyEvents();
    }

    private void CheckEditorKeyBinds()
    {
        // keyboard shortcuts with ctrl
        if (EditModeManagerOther.Instance.Playing) return;
        
        if (KeyBinds.GetKeyBindDown("Editor_SaveLevel")) SaveSystem.SaveCurrentLevel();

        // paste
        if (!CopyManager.Instance.Pasting && KeyBinds.GetKeyBind("Editor_Paste")) StartCoroutine(CopyManager.Instance.PasteCoroutine());
    }

    private static void CheckEditModeRotation()
    {
        // rotate if current edit mode is field and rotatable
        // FieldMode? fieldType = (FieldMode?)EditModeManagerOther.Instance.CurrentEditMode.TryConvertTo<EditMode, FieldMode>();
        EditMode currentEditMode = EditModeManagerOther.Instance.CurrentEditMode;

        if (currentEditMode.Attributes.IsField 
            || !((FieldMode)currentEditMode).IsRotatable 
            || !KeyBinds.GetKeyBindDown("Editor_Rotate")) return;
        
        EditModeManagerOther.Instance.EditRotation = (EditModeManagerOther.Instance.EditRotation - 90) % 360;

        if (SelectionManager.Instance.Selecting) SelectionManager.UpdatePreviewRotation();
    }

    private static void CheckTeleportPlayer()
    {
        // teleport player to mouse pos
        if (!LevelSessionManager.Instance.IsEdit || !EditModeManagerOther.Instance.Playing || !KeyBinds.GetKeyBindDown("Editor_TeleportPlayer")) return;
        
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
        foreach (EditMode editMode in EditModeManager.Instance.AllEditModes)
        {
            if(KeyBinds.GetKeyBindDown(editMode.KeyboardShortcut)) EditModeManagerOther.Instance.CurrentEditMode = editMode;
        }
    }
}