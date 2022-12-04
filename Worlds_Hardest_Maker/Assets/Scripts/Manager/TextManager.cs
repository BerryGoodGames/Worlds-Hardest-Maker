using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; } // singleton

    #region Text References
    [Header("Text References")]
    public TMPro.TMP_Text EditModeText;
    public TMPro.TMP_Text SelectingText;
    public TMPro.TMP_Text DeathText;
    public TMPro.TMP_Text Timer;
    public TMPro.TMP_Text CoinText;
    [Header("Colors")]
    public Color cheatedTimerColor;
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
        GameManager.Instance.OnPlay += StartTimer;
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
        catch (System.Exception)
        {
            // no player placed
            playerDeaths = "-";
            playerCoinsCollected = "-";
        }

        // set edit mode text ui
        Instance.EditModeText.text = $"Edit: {GameManager.Instance.CurrentEditMode.GetUIString()}";
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
        if(timerCoroutine != null)
            StopCoroutine(timerCoroutine);
    }

    public void FinishTimer()
    {
        StopTimer();

        if (!GameManager.Instance.Cheated)
            Timer.color = finishedTimerColor;
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

            while(timerSeconds >= 60)
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
        string[] splitted = timerSeconds.ToString().Split('.');

        string seconds;
        string milliseconds;
        string minutes = timerMinutes.ToString();
        string hours = timerHours.ToString();

        if (splitted.Length > 1)
        {
            seconds = splitted[0];
            milliseconds = splitted[1];
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
