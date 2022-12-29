using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; } // singleton

    #region Text References

    [FormerlySerializedAs("EditModeText")] [Header("Text References")] public TMP_Text editModeText;
    [FormerlySerializedAs("SelectingText")] public TMP_Text selectingText;
    [FormerlySerializedAs("DeathText")] public TMP_Text deathText;
    [FormerlySerializedAs("Timer")] public TMP_Text timer;
    [FormerlySerializedAs("CoinText")] public TMP_Text coinText;
    [Header("Colors")] public Color cheatedTimerColor;
    public Color finishedTimerColor;

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
            playerDeaths = currentPlayer.deaths;
            playerCoinsCollected = currentPlayer.coinsCollected.Count;
        }
        catch (Exception)
        {
            // no player placed
            playerDeaths = "-";
            playerCoinsCollected = "-";
        }

        // set edit mode text ui
        Instance.editModeText.text = $"Edit: {EditModeManager.Instance.CurrentEditMode.GetUIString()}";
        Instance.selectingText.text = $"Selecting: {SelectionManager.Instance.Selecting}";
        Instance.deathText.text = $"Deaths: {playerDeaths}";
        Instance.coinText.text = $"Coins: {playerCoinsCollected}/{CoinManager.Instance.TotalCoins}";
    }

    public void StartTimer()
    {
        StopTimer();

        timer.color = Color.black;
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

        if (!GameManager.Instance.Cheated)
            timer.color = finishedTimerColor;
    }

    private IEnumerator DoTimer()
    {
        timerSeconds = 0;
        timerMinutes = 0;
        timerHours = 0;
        timer.text = GetTimerTime();
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

            timer.text = GetTimerTime();
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