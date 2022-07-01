using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// controlling key events and managing keyboard shortcuts
/// attach to game manager
/// </summary>
public class KeyEvents : MonoBehaviour
{
    void Update()
    {
        // toggle playing
        if (Input.GetKeyDown(KeyCode.Space)) GameManager.TogglePlay();

        // toggle menu
        if(Input.GetKeyDown(KeyCode.Escape)) GameManager.Instance.Menu.SetActive(!GameManager.Instance.Menu.activeSelf);
        
#if UNITY_EDITOR
            KeyCode ctrl = KeyCode.Tab;
#else
            KeyCode ctrl = KeyCode.LeftControl;
#endif

        // keyboard shortcuts with ctrl
        if (Input.GetKey(ctrl) && !GameManager.Instance.Playing)
        {
            if (Input.GetKeyDown(KeyCode.S)) SaveSystem.SaveCurrentLevel();
            if (Input.GetKeyDown(KeyCode.O)) GameManager.LoadLevel();
            if (Input.GetKeyDown(KeyCode.C)) GameManager.ClearLevel();
        }

        // check edit mode toggling if no ctrl and not playing
        if (!Input.GetKey(KeyCode.Tab) && !GameManager.Instance.Playing && Input.anyKeyDown) CheckEditModeKeyEvents();

        // toggle filling
        if (Input.GetKeyDown(KeyCode.F)) GameManager.Instance.Filling = !GameManager.Instance.Filling;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>list of keyboard shortcuts for edit modes</returns>
    public static Dictionary<KeyCode[], GameManager.EditMode> GetKeyboardShortcuts()
    {
        Dictionary<KeyCode[], GameManager.EditMode> keys = new()
        {
            { new KeyCode[] { KeyCode.D }, GameManager.EditMode.DELETE_FIELD },
            { new KeyCode[] { KeyCode.W }, GameManager.EditMode.WALL_FIELD },
            { new KeyCode[] { KeyCode.S }, GameManager.EditMode.START_FIELD },
            { new KeyCode[] { KeyCode.G }, GameManager.EditMode.GOAL_FIELD },
            { new KeyCode[] { KeyCode.O }, GameManager.EditMode.ONE_WAY_FIELD },
            { new KeyCode[] { KeyCode.P }, GameManager.EditMode.PLAYER },
            { new KeyCode[] { KeyCode.B }, GameManager.EditMode.BALL_DEFAULT },
            { new KeyCode[] { KeyCode.C }, GameManager.EditMode.COIN },
            { new KeyCode[] { KeyCode.K }, GameManager.EditMode.GRAY_KEY },
            { new KeyCode[] { KeyCode.R, KeyCode.K }, GameManager.EditMode.RED_KEY },
            { new KeyCode[] { KeyCode.G, KeyCode.K }, GameManager.EditMode.GREEN_KEY },
            { new KeyCode[] { KeyCode.B, KeyCode.K }, GameManager.EditMode.BLUE_KEY },
            { new KeyCode[] { KeyCode.Y, KeyCode.K }, GameManager.EditMode.YELLOW_KEY },
            { new KeyCode[] { KeyCode.B, KeyCode.C }, GameManager.EditMode.BALL_CIRCLE },
            { new KeyCode[] { KeyCode.S, KeyCode.G }, GameManager.EditMode.START_AND_GOAL_FIELD },
            { new KeyCode[] { KeyCode.H, KeyCode.C }, GameManager.EditMode.CHECKPOINT_FIELD }
        };
        return keys;
    }

    private void CheckEditModeKeyEvents()
    {
        // get every user shortcut for switching edit mode
        Dictionary<KeyCode[], GameManager.EditMode> keyboardShortcuts = GetKeyboardShortcuts();

        // check every event and set edit mode accordingly
        foreach(KeyValuePair<KeyCode[], GameManager.EditMode> shortcut in keyboardShortcuts)
        {
            bool combinationPressed = true;
            foreach(KeyCode shortcutKey in shortcut.Key)
            {
                if (!Input.GetKey(shortcutKey))
                {
                    combinationPressed = false;
                    break;
                }
            }

            if (combinationPressed)
            {
                GameManager.SetEditMode(shortcut.Value);
            }
        }
    }
}
