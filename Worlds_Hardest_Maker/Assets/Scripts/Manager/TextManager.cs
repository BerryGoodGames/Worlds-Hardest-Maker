using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; } // singleton

    #region Text References

    [Header("Text References")] public TMP_Text EditModeText;

    public TMP_Text SelectingText;

    public TMP_Text DeathText;
    public TMP_Text Timer;
    public TMP_Text CoinText;

    [field: Header("Colors")] [field: SerializeField] public Color CheatedTimerColor { get; private set; }
    [field: SerializeField] public Color FinishedTimerColor { get; private set; }
    [field: SerializeField] public Color TimerDefaultColor { get; private set; }

    #endregion

    private float timerSeconds;
    private Coroutine timerCoroutine;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        EditModeManager.Instance.OnPlay += StartTimer;
        PlayerManager.Instance.OnWin += FinishTimer;
    }

    private void LateUpdate()
    {
        object playerDeaths;
        object playerCoinsCollected;

        try
        {
            PlayerController currentPlayer = PlayerManager.GetPlayer().GetComponent<PlayerController>();
            playerDeaths = currentPlayer.Deaths;
            playerCoinsCollected = currentPlayer.CoinsCollected.Count;
        }
        catch (Exception)
        {
            // no player placed
            playerDeaths = "-";
            playerCoinsCollected = "-";
        }

        // set edit mode text ui
        Instance.EditModeText.text = $"Edit: {EditModeManager.Instance.CurrentEditMode.GetUIString()}";
        Instance.SelectingText.text = $"Selecting: {SelectionManager.Instance.Selecting}";
        Instance.DeathText.text = $"Deaths: {playerDeaths}";
        Instance.CoinText.text = $"Coins: {playerCoinsCollected}/{CoinManager.Instance.TotalCoins}";
    }

    public void StartTimer()
    {
        StopTimer();

        Timer.color = TimerDefaultColor;
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
            Timer.color = FinishedTimerColor;
    }

    private IEnumerator DoTimer()
    {
        timerSeconds = 0;

        Timer.text = GetTimerTime();
        while (true)
        {
            timerSeconds += Time.deltaTime;

            Timer.text = GetTimerTime();

            yield return null;
        }
    }

    private string GetTimerTime()
    {
        TimeSpan t = TimeSpan.FromSeconds(timerSeconds);
        return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}";
    }
}