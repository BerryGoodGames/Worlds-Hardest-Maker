using System;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; set; }

    private static readonly int pickedUp = Animator.StringToHash("PickedUp");
    private static readonly int playingString = Animator.StringToHash("Playing");

    private bool cheated;

    public bool Cheated
    {
        get => cheated;
        set
        {
            cheated = value;
            TextManager.Instance.timer.color = cheated ? TextManager.Instance.cheatedTimerColor : Color.black;
        }
    }

    public Action OnGameQuit;

    #region Methods

    public void TogglePlay(bool playSoundEffect = true)
    {
        if (ReferenceManager.Instance.menu.activeSelf) return;

        if (EditModeManager.Instance.Playing) SwitchToEdit(playSoundEffect);
        else SwitchToPlay();

        foreach (BarTween tween in BarTween.tweenList)
        {
            tween.SetPlay(EditModeManager.Instance.Playing);
        }
    }

    public static void SwitchToPlay()
    {
        foreach (Transform player in ReferenceManager.Instance.playerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            if (MultiplayerManager.Instance.Multiplayer && !controller.photonView.IsMine) continue;

            controller.currentFields.Clear();
            controller.currentGameState = null;
            controller.deaths = 0;
        }

        EditModeManager.Instance.Playing = true;

        AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(false);

        // disable placement preview
        ReferenceManager.Instance.placementPreview.SetActive(false);

        // disable windows
        ReferenceManager.Instance.ballWindows.SetActive(false);

        Animator anim;

        if (AnchorManagerOld.Instance.SelectedAnchor != null)
        {
            // disable anchor lines
            AnchorManagerOld.Instance.selectedPathControllerOld.drawLines = false;
            AnchorManagerOld.Instance.selectedPathControllerOld.ClearLines();

            // disable all anchor sprites / outlines
            foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
            {
                anim = anchor.GetComponentInChildren<Animator>();
                anim.SetBool(playingString, true);
            }
        }

        // activate coin animations
        foreach (Transform coin in ReferenceManager.Instance.coinContainer)
        {
            anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, true);
            anim.SetBool(pickedUp, coin.GetChild(0).GetComponent<CoinController>().pickedUp);
        }

        // activate key animations
        foreach (Transform key in ReferenceManager.Instance.keyContainer)
        {
            anim = key.GetComponent<Animator>();
            anim.SetBool(playingString, true);
            anim.SetBool(pickedUp, key.GetChild(0).GetComponent<KeyController>().pickedUp);
        }

        // camera jumps to last player if its not on screen
        if (Camera.main != null) Camera.main.GetComponent<JumpToEntity>().Jump(true);
        EditModeManager.Instance.OnPlay?.Invoke();

        // close level settings panel if open
        LevelSettingsPanelTween lspt =
            ReferenceManager.Instance.levelSettingsPanel.GetComponent<LevelSettingsPanelTween>();
        if (lspt.open) lspt.Toggle();
    }

    public static void SwitchToEdit(bool playSoundEffect = true)
    {
        EditModeManager.Instance.Playing = false;

        if (playSoundEffect) AudioManager.Instance.Play("Bell");
        AudioManager.Instance.MusicFiltered(true);

        ResetGame();

        // enable placement preview and place it at mouse
        ReferenceManager.Instance.placementPreview.SetActive(true);
        ReferenceManager.Instance.placementPreview.transform.position =
            FollowMouse.GetCurrentMouseWorldPos(ReferenceManager.Instance.placementPreview.GetComponent<FollowMouse>()
                .worldPosition);

        // enable windows
        // if (EditModeManager.Instance.CurrentEditMode is EditMode.ANCHOR or EditMode.BALL)
        //     ReferenceManager.Instance.ballWindows.SetActive(true);

        Animator anim;

        if (AnchorManagerOld.Instance.SelectedAnchor != null)
        {
            // enable anchor lines
            AnchorManagerOld.Instance.selectedPathControllerOld.drawLines = true;
            AnchorManagerOld.Instance.selectedPathControllerOld.DrawLines();

            // enable all anchor sprites / outlines
            foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
            {
                anim = anchor.GetComponentInChildren<Animator>();
                anim.SetBool(playingString, false);
            }
        }

        // reset Anchors
        foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("Anchor"))
        {
            anchor.transform.GetChild(0).GetComponent<PathControllerOld>().ResetState();
        }

        // deactivate coin animations
        foreach (Transform coin in ReferenceManager.Instance.coinContainer.transform)
        {
            coin.GetChild(0).GetComponent<CoinController>().pickedUp = false;

            anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUp, false);
        }

        // deactivate key animations
        foreach (Transform key in ReferenceManager.Instance.keyContainer.transform)
        {
            key.GetChild(0).GetComponent<KeyController>().pickedUp = false;

            anim = key.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUp, false);
        }

        // remove game states from players
        foreach (Transform player in ReferenceManager.Instance.playerContainer.transform)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            controller.currentGameState = null;
        }

        EditModeManager.Instance.OnEdit?.Invoke();
    }

    /// <summary>
    ///     resets every field and entity to its starting state
    ///     used when switched to edit mode
    /// </summary>
    public static void ResetGame()
    {
        // reset players
        foreach (GameObject player in PlayerManager.GetPlayers())
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (MultiplayerManager.Instance.Multiplayer && !controller.photonView.IsMine) continue;
            controller.DieNormal();
        }


        // reset balls
        foreach (Transform ball in ReferenceManager.Instance.ballDefaultContainer)
        {
            GameObject ballObject = ball.GetChild(0).gameObject;
            BallDefaultController defaultController = ballObject.GetComponent<BallDefaultController>();

            ballObject.transform.position = defaultController.startPosition;
        }

        foreach (Transform ball in ReferenceManager.Instance.ballCircleContainer)
        {
            GameObject ballObject = ball.GetChild(0).gameObject;
            BallCircleController controller = ballObject.GetComponent<BallCircleController>();

            controller.currentAngle = controller.startAngle;
            controller.UpdateAnglePos();
        }

        // reset coins
        foreach (Transform coin in ReferenceManager.Instance.coinContainer)
        {
            Animator anim = coin.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUp, false);
        }

        // reset keys
        foreach (Transform key in ReferenceManager.Instance.keyContainer)
        {
            Animator anim = key.GetComponent<Animator>();
            anim.SetBool(playingString, false);
            anim.SetBool(pickedUp, false);
        }

        foreach (GameObject player in PlayerManager.GetPlayers())
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.coinsCollected.Clear();
            controller.keysCollected.Clear();
        }

        // reset checkpoints
        foreach (Transform field in ReferenceManager.Instance.fieldContainer)
        {
            if (field.CompareTag("CheckpointField"))
            {
                CheckpointTween anim = field.GetComponent<CheckpointTween>();
                anim.Deactivate();

                CheckpointController controller = field.GetComponent<CheckpointController>();
                controller.activated = false;
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
        Instance.OnGameQuit?.Invoke();

        Application.Quit();
    }

    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}