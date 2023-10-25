using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    private static TextManager Instance { get; set; } // singleton

    #region Text References

    [Header("Text References")] public TMP_Text EditModeText;

    public TMP_Text SelectingText;

    public TMP_Text DeathText;
    public TMP_Text CoinText;
    #endregion

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
}