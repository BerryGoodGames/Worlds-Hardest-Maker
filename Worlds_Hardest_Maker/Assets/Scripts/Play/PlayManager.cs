using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; private set; }
    
    // public event Action OnLevelReset = () => { };
    // public event Action OnSwitchToPlay = () => { };
    // public event Action OnSwitchToEdit = () => { };
    // public event Action OnPlaytest = () => { };
    // public event Action OnToggle = () => { };
    // public event Action OnPlaySceneSetup = () => { };
    
    private EventBus eventBus;
    
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
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
    }
    
    private void OnKonamiStateChanged(KonamiStateChangedEvent evt)
    {
        Cheated = true;
    }
    
    public void TogglePlay(bool playtest)
    {
        if (ReferenceManager.Instance.Menu.activeSelf) return;
        
        LevelSessionEditManager.Instance.Playing = !LevelSessionEditManager.Instance.Playing;
        LevelSessionEditManager.Instance.InPlaytest = LevelSessionEditManager.Instance.Playing && playtest;
        
        if (LevelSessionEditManager.Instance.Playing)
        {
            eventBus.Fire(new SwitchToPlayEvent());
        }
        else
        {
            eventBus.Fire(new SwitchToEditEvent());
        }
        
        if (LevelSessionEditManager.Instance.InPlaytest) eventBus.Fire(new StartPlaytestEvent());
        
        eventBus.Fire(new TogglePlayEditEvent());
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
        
        eventBus.Subscribe<SwitchToEditEvent>(_ =>
        {
            Cheated = false;
            FieldManager.ApplySafeFieldsColor(false);
        });
        
        eventBus.Subscribe<SwitchToPlayEvent>(_ =>
        {
            if (SettingsManager.Instance.OneColorSafeFields) FieldManager.ApplySafeFieldsColor(true);
        });
        
        LevelCompleteManager.Instance.OnPlayAgain += RestartLevel;
        
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
            
            eventBus.Fire(new SetupPlaySceneEvent());
        }
    }
    
    public void RestartLevel()
    {
        // reset game
        eventBus.Fire(new ResetLevelEvent());
        
        // start again
        if (PlayerManager.Instance.Player != null) PlayerManager.Instance.Player.Setup();
        AnchorManager.Instance.StartExecuting();
        CoinManager.Instance.ActivateAnimations();
        KeyManager.Instance.ActivateAnimations();
        
        // close menu
        ReferenceManager.Instance.MenuTween.SetVisible(false);
    }
    
    private void OnDestroy()
    {
        LevelCompleteManager.Instance.OnPlayAgain -= RestartLevel;
        eventBus.Unsubscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
    }
}