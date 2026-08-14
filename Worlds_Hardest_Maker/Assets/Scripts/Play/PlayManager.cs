using System.Collections;
using MyBox;
using UnityEngine;
using VContainer;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; private set; }

    [SerializeField] [InitializationField] [MustBeAssigned] private TimerController timerController;
    [SerializeField] [InitializationField] [MustBeAssigned] private BarTween infobarPlayTween;
    
    private EventBus eventBus;
    
    private bool cheated;
    
    public bool Cheated
    {
        get => cheated;
        set
        {
            cheated = value;
            timerController.Text.color =
                cheated
                    ? timerController.CheatedTimerColor
                    : timerController.TimerDefaultColor;
        }
    }
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
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
        
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);
        
        return;
        
        IEnumerator SetupPlayScene()
        {
            LevelSessionEditManager.Instance.Playing = true;
            LevelSessionEditManager.Instance.InPlaytest = true;
            
            yield return new WaitForEndOfFrame();
            
            infobarPlayTween.SetPlay(true);
            
            if (PlayerManager.Instance.Player != null) PlayerManager.Instance.Player.Setup();
            
            AnchorManager.Instance.StartExecuting();
            CoinManager.Instance.ActivateAnimations();
            KeyManager.Instance.ActivateAnimations();
            
            timerController.StartTimer();
            
            eventBus.Fire(new SetupPlaySceneEvent());
        }
    }
    
    private void OnKonamiStateChanged(KonamiStateChangedEvent evt)
    {
        Cheated = true;
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt)
    {
        Cheated = false;
        FieldManager.ApplySafeFieldsColor(false);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt)
    {
        if (SettingsManager.Instance.OneColorSafeFields)
        {
            FieldManager.ApplySafeFieldsColor(true);
        }
    }
    
    private void OnPlayAgain(PlayAgainEvent evt) => RestartLevel();
    
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
        eventBus.Unsubscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
    }
}