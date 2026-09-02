using System;
using MyBox;
using TMPro;
using UnityEngine;
using VContainer;

public class TextManager : MonoBehaviour
{
    private static TextManager Instance { get; set; } // singleton
    
    [Inject] private IPlayerProvider playerProvider;
    [Inject] private ICoinManager coinManager;
    
    #region Text References
    
    [Header("Text References")] [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Text editModeText;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Text deathText;
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Text coinText;
    
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
            PlayerController currentPlayer = playerProvider.Player;
            playerDeaths = currentPlayer.Deaths;
            playerCoinsCollected = coinManager.CollectedCoins.Count;
        }
        catch (Exception)
        {
            // no player placed
            playerDeaths = "-";
            playerCoinsCollected = "-";
        }
        
        deathText.text = $"Deaths: {playerDeaths}";
        coinText.text = $"Coins: {playerCoinsCollected}/{coinManager.CoinsNeededFinal}";
        
        if (!LevelSessionManager.Instance.IsEdit) return;
        
        // set edit mode text ui
        editModeText.text = $"Edit: {LevelSessionEditManager.Instance.CurrentEditMode.UIString}";
    }
}