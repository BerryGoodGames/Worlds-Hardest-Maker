using System;
using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public partial class LevelCompleteManager : MonoBehaviour
{
    public static LevelCompleteManager Instance { get; private set; }
    
    [SerializeField] [Required] private Canvas levelCompleteCanvas;
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
        
        levelCompleteCanvas.gameObject.SetActive(true);
        FillStats();
    }
    
    private void FillStats()
    {
        string levelName = LevelSessionManager.IsSessionFromEditor ? "Could not find level name because of no transition" : LevelSessionManager.Instance.LoadedLevelData.Info.Name;
        uint deathCount = LevelSessionManager.Instance.Deaths;
        TimeSpan time = LevelSessionManager.Instance.PlayRunTime;
        TimeSpan? personalBest = LevelSessionManager.Instance.BestCompletionTime;
        
        levelNameText.text = levelName;
        deathCountText.text = deathCount.ToString();
        timeText.text = Utils.GetTimerString(time);
        
        bool hasNewPB = personalBest == null || (TimeSpan)personalBest >= time;
        
        pbLabel.gameObject.SetActive(!hasNewPB);
        pbText.gameObject.SetActive(!hasNewPB);
        newPBText.gameObject.SetActive(hasNewPB);
        
        if (!hasNewPB)
        {
            pbText.text = Utils.GetTimerString((TimeSpan)personalBest);
        }
    }
    
    public void OnPlayAgainClicked()
    {
        levelCompleteCanvas.gameObject.SetActive(false);
        
        OnPlayAgain.Invoke();
    }
    
    public void OnReplayClicked()
    {
        levelCompleteCanvas.gameObject.SetActive(false);
        
        OnReplay.Invoke();
    }
    
    private void OnFinishReplay()
    {
        StartCoroutine(Wait(0.7f));
        
        return;
        
        IEnumerator Wait(float duration)
        {
            yield return new WaitForSeconds(duration);
            
            levelCompleteCanvas.gameObject.SetActive(true);
            
            PlayerRecordingManager.Instance.IsReplaying = false;
        }
    }
    
    private void OnDestroy()
    {
        PlayerManager.Instance.OnWin -= OnWin;
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
