using System;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

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
    
    public event Action OnPlayAgain = () => { };
    public event Action OnReplay = () => { };

    private void Start()
    {
        PlayerManager.Instance.OnWin += OnWin;
        
        PlayerRecordingManager.Instance.OnFinishReplay += OnFinishReplay;
    }
    
    private void OnWin()
    {
        if (LevelSessionManager.Instance.IsEdit) return;
        
        PlayerRecordingManager.Instance.SetSpriteVisible(true);
        PlayerRecordingManager.Instance.SetPathVisible(true);
        
        levelCompleteCanvasTween.SetVisible(true);
        
        FillStats();
        
        StartAnimation();
    }
    
    private void FillStats()
    {
        string levelName = LevelSessionManager.IsSessionFromEditor ? "Could not find level name because of no transition" : LevelSessionManager.Instance.LoadedLevelData.Info.Name;
        TimeSpan time = LevelSessionManager.Instance.PlayRunTime;
        TimeSpan? personalBest = LevelSessionManager.Instance.BestCompletionTime;
        
        levelNameText.text = levelName;
        
        bool hasNewPB = personalBest == null || (TimeSpan)personalBest >= time;
        
        if (!hasNewPB)
        {
            pbText.text = Utils.GetTimerString((TimeSpan)personalBest);
        }
    }
    
    public void OnPlayAgainClicked()
    {
        levelCompleteCanvasTween.SetVisible(false);
        
        OnPlayAgain.Invoke();
    }
    
    public void OnReplayClicked()
    {
        levelCompleteCanvasTween.SetVisible(false);
        
        OnReplay.Invoke();
    }
    
    private void OnFinishReplay()
    {
        StartCoroutine(Wait(0.7f));
        
        return;
        
        IEnumerator Wait(float duration)
        {
            yield return new WaitForSeconds(duration);
            
            levelCompleteCanvasTween.SetVisible(true);
            
            PlayerRecordingManager.Instance.IsReplaying = false;
        }
    }
    
    private void OnDestroy()
    {
        PlayerManager.Instance.OnWin -= OnWin;
        
        DOTween.Kill(gameObject);
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
