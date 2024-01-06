using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; private set; }


    private bool cheated;

    public bool Cheated
    {
        get => cheated;
        set
        {
            cheated = value;
            ReferenceManager.Instance.TimerController.Text.color =
                cheated
                    ? ReferenceManager.Instance.TimerController.CheatedTimerColor
                    : ReferenceManager.Instance.TimerController.TimerDefaultColor;
        }
    }

    #region Methods

    public void TogglePlay()
    {
        if (ReferenceManager.Instance.Menu.activeSelf) return;

        if (EditModeManagerOther.Instance.Playing) SwitchToEdit();
        else SwitchToPlay();

        foreach (BarTween tween in BarTween.TweenList) tween.SetPlay(EditModeManagerOther.Instance.Playing);
    }

    #region On play

    public static void SwitchToPlay()
    {
        EditModeManagerOther.Instance.Playing = true;

        AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(false);

        // disable placement preview
        ReferenceManager.Instance.PlacementPreview.gameObject.SetActive(false);

        PlayerManager.Instance.Setup();

        AnchorManager.Instance.StartExecuting();

        CoinManager.Instance.ActivateAnimations();

        KeyManager.Instance.ActivateAnimations();

        // JumpToPlayer();

        EditModeManagerOther.Instance.InvokeOnPlay();

        // hide all panels
        // TODO: abstract this
        PanelController levelSettingsPanel = ReferenceManager.Instance.LevelSettingsPanelController;
        PanelManager.Instance.SetPanelHidden(levelSettingsPanel, true);

        PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
        PanelManager.Instance.WasAnchorPanelOpen = anchorPanel.Open;
        PanelManager.Instance.SetPanelHidden(anchorPanel, true);
    }

/*
    private static void JumpToPlayer()
    {
        if (!ReferenceManager.Instance.MainCameraJumper.HasKey("Player")) return;

        ReferenceManager.Instance.MainCameraJumper.Jump("Player", onlyIfTargetOffScreen: true);
    }
*/

    #endregion

    #region On edit

    public static void SwitchToEdit()
    {
        EditModeManagerOther.Instance.Playing = false;

        AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(true);

        ResetLevel();

        EnablePreview();

        EditModeManagerOther.Instance.InvokeOnEdit();

        // show level setting / anchor panel
        bool isEditModeAnchorRelated = EditModeManagerOther.Instance.CurrentEditMode.Attributes.IsAnchorRelated;
        PanelController levelSettingsPanel = ReferenceManager.Instance.LevelSettingsPanelController;
        PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
        if (isEditModeAnchorRelated)
        {
            if (PanelManager.Instance.WasAnchorPanelOpen) PanelManager.Instance.SetPanelOpen(anchorPanel, true);
            else PanelManager.Instance.SetPanelHidden(anchorPanel, false);
        }
        else PanelManager.Instance.SetPanelHidden(levelSettingsPanel, false);
    }

    private static void EnablePreview()
    {
        // enable placement preview and place it at mouse
        ReferenceManager.Instance.PlacementPreview.gameObject.SetActive(true);
        ReferenceManager.Instance.PlacementPreview.transform.position =
            FollowMouse.GetCurrentMouseWorldPos(
                ReferenceManager.Instance.PlacementPreview
                    .GetComponent<FollowMouse>()
                    .WorldPosition
            );
    }

    /// <summary>
    ///     Resets every field and entity to its starting state
    ///     <para>Used when switched to edit mode</para>
    /// </summary>
    public static void ResetLevel()
    {
        if(PlayerManager.Instance.Player != null) PlayerManager.Instance.Player.ResetState();

        CoinManager.Instance.ResetStates();

        KeyManager.Instance.ResetStates();

        AnchorManager.Instance.ResetStates();

        // reset checkpoints
        foreach (Transform field in ReferenceManager.Instance.FieldContainer)
        {
            if (!field.CompareTag("Checkpoint")) continue;

            CheckpointTween anim = field.GetComponent<CheckpointTween>();
            anim.Deactivate();

            CheckpointController controller = field.GetComponent<CheckpointController>();
            controller.Activated = false;
        }

        // reset key doors
        string[] tags =
            { "GrayKeyDoor", "RedKeyDoor", "GreenKeyDoor", "BlueKeyDoor", "YellowKeyDoor", };

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

    public static void QuitGame() => Application.Quit();

    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // setup play scene mode
        if (!LevelSessionManager.Instance.IsEdit) StartCoroutine(SetupPlayScene());

        EditModeManagerOther.Instance.OnEdit += () => Instance.Cheated = false;

        return;

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        IEnumerator SetupPlayScene()
        {
            EditModeManagerOther.Instance.Playing = true;

            yield return new WaitForEndOfFrame();

            ReferenceManager.Instance.InfobarPlayTween.SetPlay(true);

            PlayerManager.Instance.Setup();
            AnchorManager.Instance.StartExecuting();
            CoinManager.Instance.ActivateAnimations();
            KeyManager.Instance.ActivateAnimations();

            ReferenceManager.Instance.TimerController.StartTimer();
        }
    }

    public void RestartLevel()
    {
        // reset game
        ResetLevel();

        // start again
        PlayerManager.Instance.Setup();
        AnchorManager.Instance.StartExecuting();
        CoinManager.Instance.ActivateAnimations();
        KeyManager.Instance.ActivateAnimations();

        // close menu
        ReferenceManager.Instance.MenuTween.SetVisible(false);
    }
}