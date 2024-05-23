using System;
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

    private void Start()
    {
        PlayerManager.Instance.OnWin += OnWin;
    }
    
    private void OnWin()
    {
        if (LevelSessionManager.Instance.IsEdit) return;
        
        levelCompleteCanvas.gameObject.SetActive(true);
        FillStats();
    }
    
    private void FillStats()
    {
        string levelName = LevelSessionManager.IsSessionFromEditor ? "Could not find level name because of no transition" : TransitionManager.Instance.LoadLevelPath;
        uint deathCount = LevelSessionManager.Instance.Deaths;
        TimeSpan time = LevelSessionManager.Instance.PlayTime;
        TimeSpan? personalBest = LevelSessionManager.Instance.BestCompletionTime;
        
        levelNameText.text = levelName;
        deathCountText.text = deathCount.ToString();
        timeText.text = Utils.GetTimerString(time);
        
        if (personalBest == null || (TimeSpan)personalBest > time)
        {
            pbLabel.gameObject.SetActive(false);
            pbText.gameObject.SetActive(false);
            newPBText.gameObject.SetActive(true);
        }
        else
        {
            pbText.text = Utils.GetTimerString((TimeSpan)personalBest);
        }
    }
    
    public void OnReplayClicked()
    {
        
    }
    
    private void OnDestroy()
    {
        PlayerManager.Instance.OnWin -= OnWin;
    }
}
