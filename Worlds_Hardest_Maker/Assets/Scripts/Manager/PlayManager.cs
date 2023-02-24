using System;
using UnityEngine;
using UnityEngine.Analytics;

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

    public Action GameQuit;

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

        // disable placement preview
        ReferenceManager.Instance.PlacementPreview.SetActive(false);

        StartAnchors();

        ActivateCoinKeyAnimations();

        JumpToPlayer();

        EditModeManager.Instance.InvokeOnPlay();

        // close level settings / anchor editor panel if open
        ClosePanel(ReferenceManager.Instance.LevelSettingsPanelTween);
        ClosePanel(ReferenceManager.Instance.AnchorEditorPanelTween);
    }

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
        // let anchors start executing
        foreach (Transform t in ReferenceManager.Instance.AnchorContainer)
        {
            AnchorControllerParent parent = t.GetComponent<AnchorControllerParent>();
            AnchorController anchor = parent.Child;

            anchor.StartExecuting();

            if (AnchorManager.Instance.SelectedAnchor == anchor) continue;
            anchor.Animator.SetBool(playingString, true);
        }
    }
    private static void JumpToPlayer()
    {
        if (Camera.main != null) Camera.main.GetComponent<JumpToEntity>().Jump(true);
    }
    private static void ClosePanel(PanelTween panel)
    {
        if (panel.Open) panel.Toggle();
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
        foreach (Transform key in ReferenceManager.Instance.KeyContainer)
        {
            anim = key.GetComponent<Animator>();
            anim.SetBool(playingString, true);
            anim.SetBool(pickedUpString, key.GetChild(0).GetComponent<KeyController>().PickedUp);
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

        // enable placement preview and place it at mouse
        ReferenceManager.Instance.PlacementPreview.SetActive(true);
        ReferenceManager.Instance.PlacementPreview.transform.position =
            FollowMouse.GetCurrentMouseWorldPos(ReferenceManager.Instance.PlacementPreview.GetComponent<FollowMouse>()
                .WorldPosition);

        Animator anim;

        if (AnchorManagerOld.Instance.SelectedAnchor != null)
        {
            // enable anchor lines
            AnchorManagerOld.Instance.SelectedPathControllerOld.DoDrawLines = true;
            AnchorManagerOld.Instance.SelectedPathControllerOld.DrawLines();

            // enable all anchor sprites / outlines
            foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
            {
                anim = anchor.GetComponentInChildren<Animator>();
                anim.SetBool(playingString, false);
            }
        }

        // reset Anchors
        // foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
        // {
        //     anchor.transform.GetChild(0).GetComponent<PathControllerOld>().ResetState();
        // }
        foreach (Transform t in ReferenceManager.Instance.AnchorContainer)
        {
            AnchorControllerParent parent = t.GetComponent<AnchorControllerParent>();
            AnchorController anchor = parent.Child;

            anchor.ResetExecution();
            anchor.Animator.SetBool(playingString, false);
        }

        // deactivate coin animations
        foreach (Transform coin in ReferenceManager.Instance.CoinContainer.transform)
        {
            coin.GetChild(0).GetComponent<CoinController>().PickedUp = false;

            anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUpString, false);
        }

        // deactivate key animations
        foreach (Transform key in ReferenceManager.Instance.KeyContainer.transform)
        {
            key.GetChild(0).GetComponent<KeyController>().PickedUp = false;

            anim = key.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUpString, false);
        }

        // remove game states from players
        foreach (Transform player in ReferenceManager.Instance.PlayerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            controller.CurrentGameState = null;
        }

        EditModeManager.Instance.InvokeOnEdit();
    }
    #endregion

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


        // reset balls
        foreach (Transform ball in ReferenceManager.Instance.BallDefaultContainer)
        {
            GameObject ballObject = ball.GetChild(0).gameObject;
            BallDefaultController defaultController = ballObject.GetComponent<BallDefaultController>();

            ballObject.transform.position = defaultController.StartPosition;
        }

        foreach (Transform ball in ReferenceManager.Instance.BallCircleContainer)
        {
            GameObject ballObject = ball.GetChild(0).gameObject;
            BallCircleController controller = ballObject.GetComponent<BallCircleController>();

            controller.CurrentAngle = controller.StartAngle;
            controller.UpdateAnglePos();
        }

        // reset coins
        foreach (Transform coin in ReferenceManager.Instance.CoinContainer)
        {
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUpString, false);
        }

        // reset keys
        foreach (Transform key in ReferenceManager.Instance.KeyContainer)
        {
            Animator anim = key.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUpString, false);
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
            if (field.CompareTag("CheckpointField"))
            {
                CheckpointTween anim = field.GetComponent<CheckpointTween>();
                anim.Deactivate();

                CheckpointController controller = field.GetComponent<CheckpointController>();
                controller.Activated = false;
            }
        }

        // reset key doors
        string[] tags =
            { "KeyDoorField", "RedKeyDoorField", "GreenKeyDoorField", "BlueKeyDoorField", "YellowKeyDoorField" };
        foreach (string tag in tags)
        {
            foreach (GameObject door in GameObject.FindGameObjectsWithTag(tag))
            {
                KeyDoorField comp = door.GetComponent<KeyDoorField>();
                comp.Lock(true);
            }
        }
    }

    public static void QuitGame()
    {
        Instance.GameQuit?.Invoke();

        Application.Quit();
    }

    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}