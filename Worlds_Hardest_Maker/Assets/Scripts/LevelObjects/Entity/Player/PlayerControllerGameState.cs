using System;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    private Vector2 CurrentRunStartPos
    {
        get
        {
            if (CurrentGameState != null && CurrentGameState.Checkpoint != null) return CurrentGameState.Checkpoint.transform.position;
            if (Sheet is AnchorSheet anchorSheet)
            {
                Transform sheetTransform = anchorSheet.Anchor.transform;
                Vector2 offsetPos = sheetTransform.position + Quaternion.Euler(0, 0, sheetTransform.eulerAngles.z) * SheetStartPosOffset;
                return offsetPos;
            }
            
            return StartPos;
        }
    }
    
    public void ActivateCheckpoint(CheckpointController checkpoint)
    {
        currentRunCheckpoint = checkpoint;
        
        CurrentGameState = GetGameStateNow();
        
        OnCheckpointEnter.Invoke();
        
        print("Saved game state");
    }
    
    private GameState GetGameStateNow()
    {
        coinManager.RemoveCollectedCoinNulls();
        keyManager.RemoveCollectedKeyNulls();
        
        // serialize game state
        // convert collectedCoins and collectedKeys to List<Vector2>
        List<Vector2> coinPositions = new();
        
        foreach (CoinController c in coinManager.CollectedCoins) coinPositions.Add(c.InitialPosition);
        
        List<Vector2> keyPositions = new();
        foreach (KeyController key in keyManager.CollectedKeys) keyPositions.Add(key.InitialPosition);
        
        GameState res = new()
        {
            CollectedCoins = coinPositions,
            CollectedKeys = keyPositions,
            Checkpoint = currentRunCheckpoint,
        };
        
        return res;
    }
    
    public void OnResetLevel(ResetLevelEvent evt)
    {
        DieNormal();
        coinManager.ClearCollectedCoins();
        keyManager.ClearCollectedKeys();
        CurrentGameState = null;
    }
    
    private void ResetCoinsToCurrentGameState()
    {
        foreach (CoinController coin in coinRegistry.All)
        {
            if (!coin.ShouldRespawn()) continue;
            
            coinManager.UncollectCoin(coin);
            
            coin.PickedUp = false;
            
            coin.Animator.SetBool(pickedUp, false);
        }
    }
    
    private void ApplyCurrentGameState()
    {
        // set progress from current state
        if (CurrentGameState == null) return;
        
        foreach (Vector2 coinCollectedPos in CurrentGameState.CollectedCoins)
        {
            CoinController coin = coinQueryService.FindAny(coinCollectedPos);
            if (coin == null) throw new Exception("Passed game state has null value for coin");
            
            coinManager.CollectCoin(coin);
        }
        
        foreach (Vector2 keyCollectedPos in CurrentGameState.CollectedKeys)
        {
            KeyController key = keyQueryService.FindAny(keyCollectedPos);
            if (key == null) throw new Exception("Passed game state has null value for key");
            
            keyManager.CollectKey(key);
        }
    }
    
    private void ResetKeysToCurrentGameState()
    {
        foreach (KeyController key in keyRegistry.All)
        {
            if (!key.ShouldRespawn()) continue;
            
            keyManager.UncollectKey(key);
            
            key.Collected = false;
            key.Animator.SetBool(pickedUp, false);
        }
    }
}