using System;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TextManager : MonoBehaviour
{
    private static TextManager Instance { get; set; } // singleton

    #region Text References

    [FormerlySerializedAs("EditModeText")] [Header("Text References")] [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Text editModeText;

    [FormerlySerializedAs("SelectingText")] [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Text selectingText;

    [FormerlySerializedAs("DeathText")] [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Text deathText;
    [FormerlySerializedAs("CoinText")] [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Text coinText;

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
        Instance.editModeText.text = $"Edit: {EditModeManager.Instance.CurrentEditMode.GetUIString()}";
        Instance.selectingText.text = $"Selecting: {SelectionManager.Instance.Selecting}";
        Instance.deathText.text = $"Deaths: {playerDeaths}";
        Instance.coinText.text = $"Coins: {playerCoinsCollected}/{CoinManager.Instance.TotalCoins}";
    }
}