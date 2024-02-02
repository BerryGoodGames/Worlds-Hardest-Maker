using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; private set; }

    public event Action OnLevelReset = () => { };
    public event Action OnSwitchToPlay = () => { };
    public event Action OnSwitchToEdit = () => { };
    public event Action OnToggle = () => { };

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

    public void TogglePlay()
    {
        if (ReferenceManager.Instance.Menu.activeSelf) return;

        LevelSessionEditManager.Instance.Playing = !LevelSessionEditManager.Instance.Playing;

        (LevelSessionEditManager.Instance.Playing ? OnSwitchToPlay : OnSwitchToEdit)?.Invoke();

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
            OnLevelReset.Invoke();
        };

        return;

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        IEnumerator SetupPlayScene()
        {
            LevelSessionEditManager.Instance.Playing = true;

            yield return new WaitForEndOfFrame();

            ReferenceManager.Instance.InfobarPlayTween.SetPlay(true);

            if(PlayerManager.Instance.Player != null) PlayerManager.Instance.Player.Setup();
            
            AnchorManager.Instance.StartExecuting();
            CoinManager.Instance.ActivateAnimations();
            KeyManager.Instance.ActivateAnimations();

            ReferenceManager.Instance.TimerController.StartTimer();
        }
    }

    public void RestartLevel()
    {
        // reset game
        Instance.OnLevelReset.Invoke();

        // start again
        if(PlayerManager.Instance.Player != null) PlayerManager.Instance.Player.Setup();
        AnchorManager.Instance.StartExecuting();
        CoinManager.Instance.ActivateAnimations();
        KeyManager.Instance.ActivateAnimations();

        // close menu
        ReferenceManager.Instance.MenuTween.SetVisible(false);
    }
}