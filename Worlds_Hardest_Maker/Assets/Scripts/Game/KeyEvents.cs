using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// controlling key events and managing keyboard shortcuts
/// attach to game manager
/// </summary>
public class KeyEvents : MonoBehaviour
{
    void Update()
    {
        PhotonView view = MGame.Instance.photonView;

        // toggle playing
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            MGame.Instance.TogglePlay(); 
        }

        // toggle menu
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
        {
            AlphaUITween anim = MGame.Instance.Menu.GetComponent<AlphaUITween>();
            anim.SetVisible(!anim.IsVisible());
        }

        // teleport player to mouse pos
        if(MGame.Instance.Playing && Input.GetKeyDown(KeyCode.T))
        {
            GameObject player = MPlayer.GetPlayer();
            if(player != null)
                player.GetComponent<Rigidbody2D>().position = MMouse.Instance.MouseWorldPosGrid;
        }
        
#if UNITY_EDITOR
            KeyCode ctrl = KeyCode.Tab;
#else
            KeyCode ctrl = KeyCode.LeftControl;
#endif

        // keyboard shortcuts with ctrl
        if (Input.GetKey(ctrl) && !MGame.Instance.Playing)
        {
            if (Input.GetKeyDown(KeyCode.S)) SaveSystem.SaveCurrentLevel();
#if !UNTIY_WEBGL
            if (Input.GetKeyDown(KeyCode.O)) MGame.Instance.LoadLevel();
#endif
            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    if (MGame.Instance.Multiplayer) view.RPC("ClearLevel", RpcTarget.All);
            //    else MGame.Instance.ClearLevel();
            //}
        }

        // check edit mode toggling if no ctrl and not playing
        if (!Input.GetKey(KeyCode.Tab) && !MGame.Instance.Playing && Input.anyKeyDown) CheckEditModeKeyEvents();

        // push f to fill
        MGame.Instance.Filling = Input.GetKey(MGame.Instance.FillKey);

        if (Input.GetKeyUp(MGame.Instance.FillKey))
        {
            MFill.ResetPreview();
        }

        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>list of keyboard shortcuts for edit modes</returns>
    public static Dictionary<KeyCode[], MGame.EditMode> GetKeyboardShortcuts()
    {
        Dictionary<KeyCode[], MGame.EditMode> keys = new()
        {
            { new KeyCode[] { KeyCode.D }, MGame.EditMode.DELETE_FIELD },
            { new KeyCode[] { KeyCode.W }, MGame.EditMode.WALL_FIELD },
            { new KeyCode[] { KeyCode.S }, MGame.EditMode.START_FIELD },
            { new KeyCode[] { KeyCode.G }, MGame.EditMode.GOAL_FIELD },
            { new KeyCode[] { KeyCode.O }, MGame.EditMode.ONE_WAY_FIELD },
            { new KeyCode[] { KeyCode.W, KeyCode.A }, MGame.EditMode.WATER },
            { new KeyCode[] { KeyCode.I }, MGame.EditMode.ICE },
            { new KeyCode[] { KeyCode.V }, MGame.EditMode.VOID },
            { new KeyCode[] { KeyCode.P }, MGame.EditMode.PLAYER },
            { new KeyCode[] { KeyCode.B }, MGame.EditMode.BALL_DEFAULT },
            { new KeyCode[] { KeyCode.C }, MGame.EditMode.COIN },
            { new KeyCode[] { KeyCode.K }, MGame.EditMode.GRAY_KEY },
            { new KeyCode[] { KeyCode.R, KeyCode.K }, MGame.EditMode.RED_KEY },
            { new KeyCode[] { KeyCode.G, KeyCode.K }, MGame.EditMode.GREEN_KEY },
            { new KeyCode[] { KeyCode.B, KeyCode.K }, MGame.EditMode.BLUE_KEY },
            { new KeyCode[] { KeyCode.Y, KeyCode.K }, MGame.EditMode.YELLOW_KEY },
            { new KeyCode[] { KeyCode.B, KeyCode.C }, MGame.EditMode.BALL_CIRCLE },
            { new KeyCode[] { KeyCode.H, KeyCode.C }, MGame.EditMode.CHECKPOINT_FIELD }
        };
        return keys;
    }

    private void CheckEditModeKeyEvents()
    {
        // get every user shortcut for switching edit mode
        Dictionary<KeyCode[], MGame.EditMode> keyboardShortcuts = GetKeyboardShortcuts();

        // check every event and set edit mode accordingly
        foreach(KeyValuePair<KeyCode[], MGame.EditMode> shortcut in keyboardShortcuts)
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
                MGame.Instance.CurrentEditMode = shortcut.Value;
            }
        }
    }
}
