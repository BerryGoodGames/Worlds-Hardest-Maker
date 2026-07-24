using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

public class TimerController : MonoBehaviour
{
    public TMP_Text Text;
    
    [field: Header("Colors")] [field: SerializeField] public Color CheatedTimerColor { get; private set; }
    
    [field: SerializeField] public Color FinishedTimerColor { get; private set; }
    [field: SerializeField] public Color TimerDefaultColor { get; private set; }
    
    public float TimerSeconds { get; private set; }
    private Coroutine timerCoroutine;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => StartTimer();

    private void Start()
    {
        PlayerManager.Instance.OnWin += FinishTimer;
        
        LevelCompleteManager.Instance.OnPlayAgain += StartTimer;
    }
    
    public void StartTimer()
    {
        StopTimer();
        
        Text.color = TimerDefaultColor;
        timerCoroutine = StartCoroutine(DoTimer());
    }
    
    public void StopTimer()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
    }
    
    public void ResetTimer()
    {
        TimerSeconds = 0;
        LevelSessionManager.Instance.PlayRunTime = TimeSpan.Zero;
        Text.text = GetTimerTime();
    }
    
    public void FinishTimer()
    {
        StopTimer();
        
        if (PlayManager.Instance.Cheated) return;
        
        Text.color = FinishedTimerColor;
        
        if (LevelSessionManager.Instance.IsEdit) return;
        
        LevelSessionManager.Instance.PlayRunTime = TimeSpan.FromSeconds(TimerSeconds);
        LevelSessionManager.Instance.TrySetBestTime(TimeSpan.FromSeconds(TimerSeconds));
    }
    
    private IEnumerator DoTimer()
    {
        ResetTimer();
        
        while (true)
        {
            TimerSeconds += Time.deltaTime;
            Text.text = GetTimerTime();
            
            yield return null;
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    private string GetTimerTime() => Utils.GetTimerString(TimerSeconds);
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        
        LevelCompleteManager.Instance.OnPlayAgain -= StartTimer;
    }
}