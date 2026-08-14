using System;
using System.Collections;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.PlayerRecording;

public partial class LevelCompleteManager : MonoBehaviour
{
    public static LevelCompleteManager Instance { get; private set; }
    
    [SerializeField] [MustBeAssigned] private AlphaTween levelCompleteCanvasTween;
    [SerializeField] [MustBeAssigned] private TMP_Text levelNameText;
    [SerializeField] [MustBeAssigned] private TMP_Text deathCountText;
    [SerializeField] [MustBeAssigned] private TMP_Text timeText;
    
    [SerializeField] [MustBeAssigned] private TMP_Text pbLabel;
    [SerializeField] [MustBeAssigned] private TMP_Text pbText;
    [SerializeField] [MustBeAssigned] private TMP_Text newPBText;
    
    [SerializeField] [MustBeAssigned] private TimerController timerController;

    private IRecordingService recordingService;
    private EventBus eventBus;
    
    [Inject]
    private void Construct(IRecordingService recordingService, EventBus eventBus)
    {
        this.recordingService = recordingService;
        this.eventBus = eventBus;
        
        eventBus.Subscribe<WinLevelEvent>(OnWinLevel);
        eventBus.Subscribe<FinishReplayEvent>(OnFinishReplay);
    }
    
    private void OnWinLevel(WinLevelEvent evt)
    {
        if (LevelSessionManager.Instance.IsEdit) return;
        
        recordingService.SetPathVisible(true);
        recordingService.SetOnionVisible(true);
        
        levelCompleteCanvasTween.SetVisible(true);
        
        FillStats();
        
        StartAnimation();
    }
    
    private void FillStats()
    {
        string levelName = LevelSessionManager.IsSessionFromEditor
            ? "Could not find level name because of no transition"
            : LevelSessionManager.Instance.LoadedLevelData.Info.Name;
        
        TimeSpan time = LevelSessionManager.Instance.PlayRunTime;
        TimeSpan? personalBest = LevelSessionManager.Instance.BestCompletionTime;
        
        levelNameText.text = levelName;
        
        bool hasNewPB = personalBest == null || (TimeSpan)personalBest >= time;
        
        if (!hasNewPB) pbText.text = Utils.GetTimerString((TimeSpan)personalBest);
    }
    
    public void OnPlayAgainClicked()
    {
        levelCompleteCanvasTween.SetVisible(false);
        
        eventBus.Fire(new PlayAgainEvent());
    }
    
    public void OnReplayClicked()
    {
        levelCompleteCanvasTween.SetVisible(false);
        
        eventBus.Fire(new ReplayEvent());
    }
    
    private void OnFinishReplay(FinishReplayEvent evt)
    {
        StartCoroutine(Wait(0.7f));
        
        return;
        
        IEnumerator Wait(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            
            levelCompleteCanvasTween.SetVisible(true);
            
            recordingService.StopReplay();
        }
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<WinLevelEvent>(OnWinLevel);
        eventBus.Unsubscribe<FinishReplayEvent>(OnFinishReplay);
        
        DOTween.Kill(gameObject);
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}