using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public TMP_Text Text;

    [field: Header("Colors")]
    [field: SerializeField]
    public Color CheatedTimerColor { get; private set; }

    [field: SerializeField] public Color FinishedTimerColor { get; private set; }
    [field: SerializeField] public Color TimerDefaultColor { get; private set; }

    private float timerSeconds;
    private Coroutine timerCoroutine;

    private void Start()
    {
        EditModeManager.Instance.OnPlay += StartTimer;
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
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
    }

    public void FinishTimer()
    {
        StopTimer();

        if (!PlayManager.Instance.Cheated)
            Text.color = FinishedTimerColor;
    }

    private IEnumerator DoTimer()
    {
        timerSeconds = 0;

        Text.text = GetTimerTime();
        while (true)
        {
            timerSeconds += Time.deltaTime;

            Text.text = GetTimerTime();

            yield return null;
        }
    }

    private string GetTimerTime()
    {
        TimeSpan t = TimeSpan.FromSeconds(timerSeconds);
        return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}";
    }
}
