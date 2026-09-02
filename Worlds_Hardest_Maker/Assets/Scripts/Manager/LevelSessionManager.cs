using System;
using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
///     This manager exists to assure that the session data (e.g. path of currently edited level) is always available and
///     loaded,
///     unlike TransitionManager which is only loaded when the scene is entered after main menu
/// </summary>
public class LevelSessionManager : MonoBehaviour
{
    public static LevelSessionManager Instance { get; private set; }
    
    public static bool IsSessionFromEditor => !TransitionManager.HasLoaded;
    
    [ReadOnly] public string LevelSessionPath = string.Empty;
    [ReadOnly] public LevelData LoadedLevelData;
    [ReadOnly] public LevelSessionMode LevelSessionMode;
    
    public bool IsEdit => LevelSessionMode is LevelSessionMode.Edit;
    
    public TimeSpan EditTime = TimeSpan.Zero;
    public TimeSpan PlayTime = TimeSpan.Zero;
    public TimeSpan PlayRunTime = TimeSpan.Zero;
    [Space] [ReadOnly] [OverrideLabel("Play Session Deaths")] public uint Deaths;
    [ReadOnly] [OverrideLabel("Play Session Completions")] public uint Completions;
    public TimeSpan? BestCompletionTime;
    
    public Action OnLevelLoaded = () => { Instance.BestCompletionTime = Instance.LoadedLevelData.Info.BestCompletionTime; };
    
    private EventBus eventBus;
    [Inject] private IPlayerProvider playerProvider;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<WinLevelEvent>(OnWinLevel);
    }
    
    private void OnWinLevel(WinLevelEvent evt)
    {
        if (!IsEdit) Completions++;
    }
    
    private void Update()
    {
        if (IsEdit) EditTime += TimeSpan.FromSeconds(Time.unscaledDeltaTime);
        else
        {
            PlayTime += TimeSpan.FromSeconds(Time.unscaledDeltaTime);
            if (playerProvider.HasPlayer && !playerProvider.Player.Won) PlayRunTime += TimeSpan.FromSeconds(Time.deltaTime);
        }
    }
    
    private void Start()
    {
        if (IsSessionFromEditor)
        {
            // load level from Dbg if session is from editor
            if (Dbg.Instance.AutoLoadLevel) LevelSessionPath = $"{SaveSystem.LevelSavePath}/{Dbg.Instance.LevelName}.lvl";
            
            LevelSessionMode = Dbg.Instance.EditorLevelSessionMode;
        }
        else
        {
            // pass data from TransitionManager over to LevelSessionManager
            LevelSessionPath = TransitionManager.Instance.LoadLevelPath;
            LevelSessionMode = TransitionManager.Instance.LevelSessionMode;
            
            // load user-selected level
            Coroutine loadLevelCoroutine = GameManager.Instance.LoadLevel(LevelSessionPath);
            loadLevelCoroutine.OnComplete(() => OnLevelLoaded.Invoke());
        }
        
        ConditionalObject[] conditionalObjects = FindObjectsOfType<ConditionalObject>(true);
        foreach (ConditionalObject obj in conditionalObjects)
        {
            if (obj.EditOnly && !IsEdit) Destroy(obj.gameObject);
            if (obj.PlayOnly && IsEdit) Destroy(obj.gameObject);
        }
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    public void TrySetBestTime(TimeSpan time)
    {
        if (BestCompletionTime == null || time < BestCompletionTime) BestCompletionTime = time;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<WinLevelEvent>(OnWinLevel);
    }
}