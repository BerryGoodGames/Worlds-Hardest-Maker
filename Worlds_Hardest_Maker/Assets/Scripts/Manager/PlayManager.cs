using System;
using System.Windows.Forms;
using UnityEngine;
using Application = UnityEngine.Application;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; set; }

    private static readonly int pickedUpString = Animator.StringToHash("PickedUp");
    private static readonly int playingString = Animator.StringToHash("Playing");

    private bool cheated;

    public bool Cheated
    {
        get => cheated;
        set
        {
            cheated = value;
            TextManager.Instance.Timer.color = cheated ? TextManager.Instance.CheatedTimerColor : Color.black;
        }
    }

    public Action OnGameQuit;

    #region Methods

    public void TogglePlay()
    {
        if (ReferenceManager.Instance.Menu.activeSelf) return;

        if (EditModeManager.Instance.Playing) SwitchToEdit();
        else SwitchToPlay();

        foreach (BarTween tween in BarTween.TweenList)
        {
            tween.SetPlay(EditModeManager.Instance.Playing);
        }
    }

    #region On play

    public static void SwitchToPlay()
    {
        SetupPlayers();

        EditModeManager.Instance.Playing = true;

        AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(false);

        DisablePreview();

        StartAnchors();

        ActivateCoinKeyAnimations();

        // JumpToPlayer();

        EditModeManager.Instance.InvokeOnPlay();

        // hide all panels
        PanelController levelSettingsPanel = ReferenceManager.Instance.LevelSettingsPanelController;
        PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
        PanelManager.Instance.SetPanelHidden(levelSettingsPanel, true);

        PanelManager.Instance.WasAnchorPanelOpen = anchorPanel.Open;
        PanelManager.Instance.SetPanelHidden(anchorPanel, true);
    }

    private static void DisablePreview() =>
        // disable placement preview
        ReferenceManager.Instance.PlacementPreview.gameObject.SetActive(false);

    private static void SetupPlayers()
    {
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            if (MultiplayerManager.Instance.Multiplayer && !controller.PhotonView.IsMine) continue;

            controller.CurrentFields.Clear();
            controller.CurrentGameState = null;
            controller.Deaths = 0;
        }
    }

    private static void StartAnchors()
    {
        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

        // let anchors start executing
        foreach (Transform t in ReferenceManager.Instance.AnchorContainer)
        {
            AnchorParentController parent = t.GetComponent<AnchorParentController>();
            AnchorController anchor = parent.Child;

            anchor.StartExecuting();

            anchor.SetLinesActive(false);

            if (AnchorManager.Instance.SelectedAnchor == anchor &&
                EditModeManager.Instance.CurrentEditMode.IsAnchorRelated()) continue;

            anchor.Animator.SetBool(playingString, true);
        }
    }

    private static void JumpToPlayer()
    {
        if (!ReferenceManager.Instance.MainCameraJumper.HasKey("Player")) return;

        ReferenceManager.Instance.MainCameraJumper.Jump("Player", onlyIfTargetOffScreen: true);
    }

    private static void ActivateCoinKeyAnimations()
    {
        Animator anim;

        // activate coin animations
        foreach (Transform coin in ReferenceManager.Instance.CoinContainer)
        {
            anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, true);
            anim.SetBool(pickedUpString, coin.GetChild(0).GetComponent<CoinController>().PickedUp);
        }

        // activate key animations
        foreach (KeyController key in KeyManager.Instance.Keys)
        {
            key.Animator.SetBool(playingString, true);
            key.Animator.SetBool(pickedUpString, key.PickedUp);
        }
    }

    #endregion

    #region On edit

    public static void SwitchToEdit()
    {
        EditModeManager.Instance.Playing = false;

        AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(true);

        ResetGame();

        EnablePreview();

        ResetAnchors();

        ResetCoinKeyAnimations();

        ResetPlayerGameStates();

        EditModeManager.Instance.InvokeOnEdit();

        // show level setting / anchor panel
        bool isEditModeAnchorRelated = EditModeManager.Instance.CurrentEditMode.IsAnchorRelated();
        PanelController levelSettingsPanel = ReferenceManager.Instance.LevelSettingsPanelController;
        PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
        if (isEditModeAnchorRelated)
        {
            if (PanelManager.Instance.WasAnchorPanelOpen)
            {
                PanelManager.Instance.SetPanelOpen(anchorPanel, true);
            }
            else
            {
                PanelManager.Instance.SetPanelHidden(anchorPanel, false);
            }
        }
        else
        {
            PanelManager.Instance.SetPanelHidden(levelSettingsPanel, false);
        }
    }

    private static void ResetPlayerGameStates()
    {
        // remove game states from players
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            controller.CurrentGameState = null;
        }
    }

    private static void EnablePreview()
    {
        // enable placement preview and place it at mouse
        ReferenceManager.Instance.PlacementPreview.gameObject.SetActive(true);
        ReferenceManager.Instance.PlacementPreview.transform.position =
            FollowMouse.GetCurrentMouseWorldPos(ReferenceManager.Instance.PlacementPreview
                .GetComponent<FollowMouse>()
                .WorldPosition);
    }

    private static void ResetAnchors()
    {
        // reset anchors
        foreach (Transform t in ReferenceManager.Instance.AnchorContainer)
        {
            AnchorParentController parent = t.GetComponent<AnchorParentController>();
            AnchorController anchor = parent.Child;

            anchor.ResetExecution();
            anchor.Animator.SetBool(playingString, false);

            if (AnchorManager.Instance.SelectedAnchor == anchor &&
                EditModeManager.Instance.CurrentEditMode.IsAnchorRelated()) anchor.SetLinesActive(true);
        }
    }

    private static void ResetCoinKeyAnimations()
    {
        Animator anim;

        // deactivate coin animations
        foreach (Transform coin in ReferenceManager.Instance.CoinContainer.transform)
        {
            coin.GetChild(0).GetComponent<CoinController>().PickedUp = false;

            anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUpString, false);
        }

        // deactivate key animations
        foreach (KeyController key in KeyManager.Instance.Keys)
        {
            key.PickedUp = false;

            key.Animator.SetBool(playingString, false);
            key.Animator.SetBool(pickedUpString, false);
        }
    }

    /// <summary>
    ///     Resets every field and entity to its starting state
    ///     <para>Used when switched to edit mode</para>
    /// </summary>
    public static void ResetGame()
    {
        // reset players
        foreach (GameObject player in PlayerManager.GetPlayers())
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (MultiplayerManager.Instance.Multiplayer && !controller.PhotonView.IsMine) continue;
            controller.DieNormal();
        }

        // reset coins
        foreach (Transform coin in ReferenceManager.Instance.CoinContainer)
        {
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUpString, false);
        }

        // reset keys
        foreach (KeyController key in KeyManager.Instance.Keys)
        {
            key.Animator.SetBool(playingString, false);
            key.Animator.SetBool(pickedUpString, false);
        }

        foreach (GameObject player in PlayerManager.GetPlayers())
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.CoinsCollected.Clear();
            controller.KeysCollected.Clear();
        }

        // reset checkpoints
        foreach (Transform field in ReferenceManager.Instance.FieldContainer)
        {
            if (!field.CompareTag("CheckpointField")) continue;

            CheckpointTween anim = field.GetComponent<CheckpointTween>();
            anim.Deactivate();

            CheckpointController controller = field.GetComponent<CheckpointController>();
            controller.Activated = false;
        }

        // reset key doors
        string[] tags =
            { "KeyDoorField", "RedKeyDoorField", "GreenKeyDoorField", "BlueKeyDoorField", "YellowKeyDoorField" };
        foreach (string tag in tags)
        {
            foreach (GameObject door in GameObject.FindGameObjectsWithTag(tag))
            {
                KeyDoorFieldController comp = door.GetComponent<KeyDoorFieldController>();
                comp.SetLocked(true);
            }
        }
    }

    #endregion

    public static void QuitGame()
    {
        Instance.OnGameQuit?.Invoke();

        Application.Quit();
    }

    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}