using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; } // singleton

    #region Text References

    [FormerlySerializedAs("editModeText")] [Header("Text References")]
    public TMP_Text EditModeText;

    [FormerlySerializedAs("selectingText")]
    public TMP_Text SelectingText;

    [FormerlySerializedAs("deathText")] public TMP_Text DeathText;
    [FormerlySerializedAs("timer")] public TMP_Text Timer;
    [FormerlySerializedAs("coinText")] public TMP_Text CoinText;

    [FormerlySerializedAs("cheatedTimerColor")] [Header("Colors")]
    public Color CheatedTimerColor;

    [FormerlySerializedAs("finishedTimerColor")]
    public Color FinishedTimerColor;

    #endregion

    private float timerSeconds;
    private float timerMinutes;
    private float timerHours;
    private Coroutine timerCoroutine;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        EditModeManager.Instance.Play += StartTimer;
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

        Timer.color = Color.black;
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
        timerMinutes = 0;
        timerHours = 0;
        Timer.text = GetTimerTime();
        while (true)
        {
            timerSeconds += Time.deltaTime;

            while (timerSeconds >= 60)
            {
                timerMinutes++;
                timerSeconds -= 60;
            }

            while (timerMinutes >= 60)
            {
                timerHours++;
                timerMinutes -= 60;
            }

            Timer.text = GetTimerTime();
            yield return null;
        }
    }

    private string GetTimerTime()
    {
        string[] split = timerSeconds.ToString().Split('.');

        string seconds;
        string milliseconds;
        string minutes = timerMinutes.ToString();
        string hours = timerHours.ToString();

        if (split.Length > 1)
        {
            seconds = split[0];
            milliseconds = split[1];
        }
        else
        {
            seconds = timerSeconds.ToString();
            milliseconds = "000";
        }

        seconds = seconds.PadLeft(2, '0');
        milliseconds = milliseconds.PadLeft(3, '0');
        minutes = minutes.PadLeft(2, '0');
        hours = hours.PadLeft(2, '0');

        if (milliseconds.Length > 3)
            milliseconds = milliseconds[..3];

        return $"{hours}:{minutes}:{seconds}.{milliseconds}";
    }
}