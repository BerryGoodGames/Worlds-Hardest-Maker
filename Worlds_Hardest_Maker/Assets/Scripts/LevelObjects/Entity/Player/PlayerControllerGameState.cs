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
            if (Sheet == null) return StartPos;
            
            Transform sheetTransform = Sheet.transform;
            Vector2 offsetPos = sheetTransform.position + Quaternion.Euler(0, 0, sheetTransform.eulerAngles.z) * SheetStartPosOffset;
            return offsetPos;
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
        CoinManager.Instance.CollectedCoins.RemoveAll(e => e == null);
        KeyManager.Instance.CollectedKeys.RemoveAll(e => e == null);
        
        // serialize game state
        // convert collectedCoins and collectedKeys to List<Vector2>
        List<Vector2> coinPositions = new();
        
        foreach (CoinController c in CoinManager.Instance.CollectedCoins) coinPositions.Add(c.InitialPosition);
        
        List<Vector2> keyPositions = new();
        foreach (KeyController key in KeyManager.Instance.CollectedKeys) keyPositions.Add(key.InitialPosition);
        
        GameState res = new()
        {
            CollectedCoins = coinPositions,
            CollectedKeys = keyPositions,
            Checkpoint = currentRunCheckpoint,
        };
        
        return res;
    }
    
    public void ResetState()
    {
        DieNormal();
        CoinManager.Instance.CollectedCoins.Clear();
        KeyManager.Instance.CollectedKeys.Clear();
        CurrentGameState = null;
    }
    
    private void ResetCoinsToCurrentGameState()
    {
        foreach (CoinController coin in CoinManager.Instance.Coins)
        {
            if (!coin.ShouldRespawn()) continue;
            
            CoinManager.Instance.CollectedCoins.Remove(coin);
            
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
            CoinController coin = CoinManager.Instance.Get(coinCollectedPos);
            if (coin == null) throw new Exception("Passed game state has null value for coin");
            
            CoinManager.Instance.CollectedCoins.Add(coin);
        }
        
        foreach (Vector2 keyCollectedPos in CurrentGameState.CollectedKeys)
        {
            KeyController key = KeyManager.Instance.Get(keyCollectedPos);
            if (key == null) throw new Exception("Passed game state has null value for key");
            
            KeyManager.Instance.CollectedKeys.Add(key);
        }
    }
    
    private void ResetKeysToCurrentGameState()
    {
        foreach (KeyController key in KeyManager.Instance.Keys)
        {
            if (!key.ShouldRespawn()) continue;
            
            KeyManager.Instance.CollectedKeys.Remove(key);
            
            key.Collected = false;
            key.Animator.SetBool(pickedUp, false);
        }
    }
}