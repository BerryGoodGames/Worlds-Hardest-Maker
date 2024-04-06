using System;
using System.Collections;
using MyBox;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; private set; }

    public event Action OnLevelReset = () => { };
    public event Action OnSwitchToPlay = () => { };
    public event Action OnSwitchToEdit = () => { };
    public event Action OnPlaytest = () => { };
    public event Action OnToggle = () => { };
    public event Action OnPlaySceneSetup = () => { };

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

    public void TogglePlay(bool playtest)
    {
        if (ReferenceManager.Instance.Menu.activeSelf) return;

        LevelSessionEditManager.Instance.Playing = !LevelSessionEditManager.Instance.Playing;
        LevelSessionEditManager.Instance.InPlaytest = LevelSessionEditManager.Instance.Playing && playtest;

        (LevelSessionEditManager.Instance.Playing ? OnSwitchToPlay : OnSwitchToEdit)?.Invoke();

        if (LevelSessionEditManager.Instance.InPlaytest) OnPlaytest.Invoke();

        OnToggle.Invoke();
    }

    public static void QuitGame() => Application.Quit();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // setup play scene mode
        if (!LevelSessionManager.Instance.IsEdit) StartCoroutine(SetupPlayScene());

        OnSwitchToEdit += () =>
        {
            Cheated = false;
            FieldManager.ApplySafeFieldsColor(false);
        };

        OnSwitchToPlay += () =>
        {
            if (SettingsManager.Instance.OneColorSafeFields) FieldManager.ApplySafeFieldsColor(true);
        };

        return;

        IEnumerator SetupPlayScene()
        {
            LevelSessionEditManager.Instance.Playing = true;
            LevelSessionEditManager.Instance.InPlaytest = true;

            yield return new WaitForEndOfFrame();

            ReferenceManager.Instance.InfobarPlayTween.SetPlay(true);

            if (PlayerManager.Instance.Player != null) PlayerManager.Instance.Player.Setup();

            AnchorManager.Instance.StartExecuting();
            CoinManager.Instance.ActivateAnimations();
            KeyManager.Instance.ActivateAnimations();

            ReferenceManager.Instance.TimerController.StartTimer();
            
            OnPlaySceneSetup.Invoke();
        }
    }

    public void RestartLevel()
    {
        // reset game
        OnLevelReset.Invoke();

        // start again
        if (PlayerManager.Instance.Player != null) PlayerManager.Instance.Player.Setup();
        AnchorManager.Instance.StartExecuting();
        CoinManager.Instance.ActivateAnimations();
        KeyManager.Instance.ActivateAnimations();

        // close menu
        ReferenceManager.Instance.MenuTween.SetVisible(false);
    }
}