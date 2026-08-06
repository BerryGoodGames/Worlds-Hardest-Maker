using System;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using VContainer;

public partial class LevelCompleteManager : MonoBehaviour
{
    public static LevelCompleteManager Instance { get; private set; }
    
    [SerializeField] [Required] private AlphaTween levelCompleteCanvasTween;
    [SerializeField] [Required] private TMP_Text levelNameText;
    [SerializeField] [Required] private TMP_Text deathCountText;
    [SerializeField] [Required] private TMP_Text timeText;
    
    [SerializeField] [Required] private TMP_Text pbLabel;
    [SerializeField] [Required] private TMP_Text pbText;
    [SerializeField] [Required] private TMP_Text newPBText;
    
    [SerializeField] [Required] private TimerController timerController;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<WinLevelEvent>(OnWinLevel);
        eventBus.Subscribe<FinishReplayEvent>(OnFinishReplay);
    }
    
    private void OnWinLevel(WinLevelEvent evt)
    {
        if (LevelSessionManager.Instance.IsEdit) return;
        
        PlayerRecordingManager.Instance.SetPathVisible(true);
        PlayerRecordingManager.Instance.SetOnionVisible(true);
        
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
            
            PlayerRecordingManager.Instance.IsReplaying = false;
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