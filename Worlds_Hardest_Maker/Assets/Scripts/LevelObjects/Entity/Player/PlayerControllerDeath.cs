using DG.Tweening;
using UnityEngine;

public partial class PlayerController
{
    public void DieNormal(string soundEffect = "Death")
    {
        if (Won) return;
        
        // default dying
        // avoid dying while in animation
        if (!InDeathAnim) DefaultDeathAnim();
        
        if (LevelSessionEditManager.Instance.IsPlaying)
        {
            // sfx and death counter
            audioService.Play(soundEffect);
        }
        
        Death();
    }
    
    public void DieVoid(Vector2 fallPosition)
    {
        if (Won) return;
        
        spriteRenderer.DOFade(0, voidFallDuration)
            .SetEase(Ease.Linear);
        
        transform.DOMove(fallPosition, voidFallDuration)
            .SetEase(Ease.OutQuint);
        
        transform.DOScale(Vector2.zero, voidFallDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(DeathAnimFinish);
        
        audioService.Play("DeathFall");
        
        Death();
    }
    
    public void DefaultDeathAnim(float? duration = null) =>
        spriteRenderer.DOFade(0, duration ?? defaultDeathFadeDuration)
            .SetEase(Ease.Linear)
            .OnComplete(DeathAnimFinish);
    
    
    /// <summary>
    ///     general method when dying in any way
    /// </summary>
    private void Death()
    {
        Rb.linearVelocity = Vector2.zero;
        Rb.simulated = false;
        InDeathAnim = true;
        
        if (LevelSessionEditManager.Instance.IsPlaying)
        {
            Deaths++;
            if (!LevelSessionManager.Instance.IsEdit) LevelSessionManager.Instance.Deaths++;
        }
        
        UpdateCoinCounterDeath();
        
        OnDeathEnter.Invoke();
        
        if (konamiService.IsKonamiActive) return;
        
        // set timer color to "not cheated", unless when hit a checkpoint
        if (!HasTeleported || CurrentGameState == null) PlayManager.Instance.Cheated = false;
        
        // reset balls to start position (if player launched them e.g. with shotgun)
        foreach (BallController ball in BallManager.Instance.BallList) ball.ResetPosition();
    }
    
    private void RevertDeathAnimation()
    {
        Transform t = transform;
        
        // cancel potential animations
        spriteRenderer.DOKill();
        t.DOKill();
        
        // reset color + scale
        Color color = spriteRenderer.color;
        color = new(color.r, color.g, color.b, 1);
        spriteRenderer.color = color;
        
        // fade out again
        if (IsAttached && LevelSessionEditManager.Instance.IsEditing)
        {
            AnchorAttachment attachment = GetComponent<AnchorAttachment>();
            AnchorAttachFade fade = attachment.Anchor.AttachFade;
            fade.FadeOut();
        }
        
        t.localScale = defaultScale;
    }
    
    private void UpdateCoinCounterDeath()
    {
        // update coin counter
        bool hasCheckpointActivated = CurrentGameState != null;
        CoinManager.Instance.CollectedCoins.Clear();
        
        if (!hasCheckpointActivated) return;
        
        foreach (Vector2 coinPos in CurrentGameState.CollectedCoins)
        {
            CoinController coin = CoinManager.Instance.Get(coinPos);
            if (coin != null) CoinManager.Instance.CollectedCoins.Add(coin);
        }
    }
    
    public void DeathAnimFinish()
    {
        // reset timer if no checkpoint activated
        bool hasCheckpointActivated = CurrentGameState != null;
        if (!hasCheckpointActivated) timerController.ResetTimer();
        
        Won = false;
        InDeathAnim = false;
        Rb.simulated = true;
        
        Vector2 spawnPos = CurrentRunStartPos;
        
        if (LevelSessionEditManager.Instance.IsEditing) spawnPos = StartPos;
        
        OnDeathEnd.Invoke();
        
        transform.position = spawnPos;
        
        RevertDeathAnimation();
        
        Camera main = Camera.main;
        if (main != null)
        {
            JumpToEntity jumpToPlayer = main.GetComponent<JumpToEntity>();
            if (jumpToPlayer.GetTarget("Player") == gameObject) jumpToPlayer.SetTarget("Player", gameObject);
        }
        
        ResetCoinsToCurrentGameState();
        ResetKeysToCurrentGameState();
        
        string[] tags =
            { "GrayKeyDoor", "RedKeyDoor", "GreenKeyDoor", "BlueKeyDoor", "YellowKeyDoor", };
        
        foreach (string tag in tags)
        {
            foreach (GameObject door in GameObject.FindGameObjectsWithTag(tag))
            {
                KeyDoorFieldController comp = door.GetComponent<KeyDoorFieldController>();
                if (!KeyManager.Instance.AllKeysCollected(comp.Color)) comp.SetLocked(true);
            }
        }
    }
}