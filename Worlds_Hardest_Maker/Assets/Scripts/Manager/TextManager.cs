using System;
using MyBox;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    private static TextManager Instance { get; set; } // singleton
    
    #region Text References
    
    [Header("Text References")] [SerializeField] [InitializationField] [Required] private TMP_Text editModeText;
    
    [SerializeField] [InitializationField] [Required] private TMP_Text deathText;
    [SerializeField] [InitializationField] [Required] private TMP_Text coinText;
    
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
            PlayerController currentPlayer = PlayerManager.Instance.Player;
            playerDeaths = currentPlayer.Deaths;
            playerCoinsCollected = CoinManager.Instance.CollectedCoins.Count;
        }
        catch (Exception)
        {
            // no player placed
            playerDeaths = "-";
            playerCoinsCollected = "-";
        }
        
        Instance.deathText.text = $"Deaths: {playerDeaths}";
        Instance.coinText.text = $"Coins: {playerCoinsCollected}/{CoinManager.Instance.CoinsNeededFinal}";
        
        if (!LevelSessionManager.Instance.IsEdit) return;
        
        // set edit mode text ui
        Instance.editModeText.text = $"Edit: {LevelSessionEditManager.Instance.CurrentEditMode.UIString}";
    }
}