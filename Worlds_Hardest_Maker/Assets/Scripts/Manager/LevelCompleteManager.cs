using System;
using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class LevelCompleteManager : MonoBehaviour
{
    [SerializeField] [Required] private Canvas levelCompleteCanvas;
    [SerializeField] [Required] private TMP_Text levelNameText;
    [SerializeField] [Required] private TMP_Text deathCountText;
    [SerializeField] [Required] private TMP_Text timeText;
    
    [SerializeField] [Required] private TMP_Text pbLabel;
    [SerializeField] [Required] private TMP_Text pbText;
    [SerializeField] [Required] private TMP_Text newPBText;
    
    [SerializeField] [Required] private TimerController timerController;

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
        
        if (personalBest == null || (TimeSpan)personalBest >= time)
        {
            pbLabel.gameObject.SetActive(false);
            pbText.gameObject.SetActive(false);
            newPBText.gameObject.SetActive(true);
        }
        else
        {
            pbLabel.gameObject.SetActive(true);
            pbText.gameObject.SetActive(true);
            newPBText.gameObject.SetActive(false);
            pbText.text = Utils.GetTimerString((TimeSpan)personalBest);
        }
    }
    
    public void OnPlayAgainClicked()
    {
        PlayManager.Instance.RestartLevel();
        CoinManager.Instance.CollectedCoins.Clear();
        KeyManager.Instance.CollectedKeys.Clear();
        
        if (PlayerManager.Instance.Player)
        {
            PlayerManager.Instance.Player.CurrentGameState = null;
            PlayerManager.Instance.Player.DefaultDeathAnim(0);
            PlayerManager.Instance.Player.Deaths = 0;
        }
        
        timerController.StartTimer();
        
        levelCompleteCanvas.gameObject.SetActive(false);
        
        PlayerRecordingManager.Instance.SetSpriteVisible(false);
        PlayerRecordingManager.Instance.SetPathVisible(false);
        
        PlayerRecordingManager.Instance.StartPlayerRecording();
        
        PlayerRecordingManager.Instance.IsReplaying = false;
    }
    
    public void OnReplayClicked()
    {
        PlayerRecordingManager.Instance.IsReplaying = true;
        
        levelCompleteCanvas.gameObject.SetActive(false);
        
        PlayerRecordingManager.Instance.SetSpriteVisible(false);
        PlayerRecordingManager.Instance.SetPathVisible(true);
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
}
