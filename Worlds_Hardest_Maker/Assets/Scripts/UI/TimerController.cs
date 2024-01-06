using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public TMP_Text Text;

    [field: Header("Colors")] [field: SerializeField] public Color CheatedTimerColor { get; private set; }

    [field: SerializeField] public Color FinishedTimerColor { get; private set; }
    [field: SerializeField] public Color TimerDefaultColor { get; private set; }

    public float TimerSeconds { get; private set; }
    private Coroutine timerCoroutine;

    private void Start()
    {
        PlayManager.Instance.OnSwitchToPlay += StartTimer;
        PlayerManager.Instance.OnWin += FinishTimer;
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
        Text.text = GetTimerTime();
    }

    public void FinishTimer()
    {
        StopTimer();

        if (PlayManager.Instance.Cheated) return;

        Text.color = FinishedTimerColor;

        if (!LevelSessionManager.Instance.IsEdit) LevelSessionManager.Instance.TrySetBestTime(TimeSpan.FromSeconds(TimerSeconds));
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

    private string GetTimerTime()
    {
        TimeSpan t = TimeSpan.FromSeconds(TimerSeconds);
        return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}";
    }
}