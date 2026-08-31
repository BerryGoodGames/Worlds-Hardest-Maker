using System.Collections;
using MyBox;
using UnityEngine;
using VContainer;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; private set; }

    [SerializeField] [InitializationField] [MustBeAssigned] private TimerController timerController;
    [SerializeField] [InitializationField] [MustBeAssigned] private BarTween infobarPlayTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject menu;
    [SerializeField] [InitializationField] [MustBeAssigned] private AlphaTween menuTween;
    
    private EventBus eventBus;
    [Inject] private IPlayerManager playerManager;
    
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
        if (menu.activeSelf) return;
        
        LevelSessionEditManager.Instance.IsPlaying = !LevelSessionEditManager.Instance.IsPlaying;
        LevelSessionEditManager.Instance.IsPlaytesting = LevelSessionEditManager.Instance.IsPlaying && playtest;
        
        if (LevelSessionEditManager.Instance.IsPlaying)
        {
            eventBus.Fire(new SwitchToPlayEvent());
        }
        else
        {
            eventBus.Fire(new SwitchToEditEvent());
        }
        
        if (LevelSessionEditManager.Instance.IsPlaytesting) eventBus.Fire(new StartPlaytestEvent());
        
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
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);
        
        return;
        
        IEnumerator SetupPlayScene()
        {
            LevelSessionEditManager.Instance.IsPlaying = true;
            LevelSessionEditManager.Instance.IsPlaytesting = true;
            
            yield return new WaitForEndOfFrame();
            
            infobarPlayTween.SetPlay(true);
            
            if (playerManager.Player != null) playerManager.Player.Setup();
            
            timerController.StartTimer();
            
            eventBus.Fire(new SetupPlaySceneEvent());
        }
    }
    
    private void OnKonamiStateChanged(KonamiStateChangedEvent evt) => Cheated = true;

    private void OnSwitchToEdit(SwitchToEditEvent evt) => Cheated = false;

    private void OnPlayAgain(PlayAgainEvent evt) => RestartLevel();
    
    public void RestartLevel()
    {
        // close menu
        menuTween.SetVisible(false);
        
        // reset game
        eventBus.Fire(new ResetLevelEvent());
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
    }
}