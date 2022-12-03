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
    #endregion

    private float timerTime;
    private Coroutine timerCoroutine;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
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
        timerCoroutine = StartCoroutine(DoTimer());
    }

    public void StopTimer()
    {
        StopCoroutine(timerCoroutine);
    }

    private IEnumerator DoTimer()
    {
        timerTime = 0;
        Timer.text = timerTime.ToString();
        while (true)
        {
            timerTime += Time.deltaTime;

            Timer.text = timerTime.ToString();
            yield return null;
        }
    }
}
