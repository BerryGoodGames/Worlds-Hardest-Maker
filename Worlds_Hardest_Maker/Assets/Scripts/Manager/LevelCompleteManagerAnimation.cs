using System;
using DG.Tweening;
using MyBox;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public partial class LevelCompleteManager
{
    [Separator("Animation settings")] [SerializeField] [Required] private TMP_Text levelNameLabel;
    [SerializeField] [Required] private TMP_Text deathCountLabel;
    [SerializeField] [Required] private TMP_Text timeLabel;
    [Space] [SerializeField] [PositiveValueOnly] private float startDelay = 0.8f;
    [SerializeField] private Ease appearEase = Ease.OutCirc;
    [SerializeField] [PositiveValueOnly] private float appearDuration = 0.8f;
    [SerializeField] [PositiveValueOnly] private float highlightDuration = 0.4f;
    [SerializeField] [PositiveValueOnly] private float highlightScale = 1.2f;
    [SerializeField] [PositiveValueOnly] private float numberAnimateDurationShort = 1f;
    [SerializeField] [PositiveValueOnly] private float numberAnimateDurationLong = 2.5f;
    
    private Sequence animationSequence;
    
    private void StartAnimation()
    {
        TMP_Text[] texts =
        {
            levelNameLabel,
            levelNameText,
            deathCountLabel,
            deathCountText,
            timeLabel,
            timeText,
            pbLabel,
            pbText,
            newPBText,
        };
        
        int deaths = (int)LevelSessionManager.Instance.Deaths;
        TimeSpan time = LevelSessionManager.Instance.PlayRunTime;
        TimeSpan? personalBest = LevelSessionManager.Instance.BestCompletionTime;
        
        float deathAnimateDuration = deaths == 0 ? 0 : deaths > 15 ? numberAnimateDurationLong : numberAnimateDurationShort;
        
        animationSequence?.Kill();
        animationSequence = DOTween.Sequence();
        
        foreach (TMP_Text text in texts) text.transform.localScale = Vector3.zero;
        
        timeText.text = Utils.GetTimerString(0);
        
        animationSequence.Append(
                levelNameLabel.transform.DOScale(Vector3.one, appearDuration)
                    .SetEase(appearEase)
                    .SetDelay(startDelay)
            )
            .Append(levelNameText.transform.DOScale(Vector3.one, appearDuration).SetEase(appearEase))
            .Append(deathCountLabel.transform.DOScale(Vector3.one, appearDuration).SetEase(appearEase))
            .Append(
                deathCountText.transform.DOScale(Vector3.one, appearDuration)
                    .SetEase(appearEase)
                    .OnComplete(() => AnimateDeathCounter(deaths, deathAnimateDuration))
            )
            .Append(
                deathCountText.transform.DOScale(Vector3.one * highlightScale, highlightDuration / 2)
                    .SetEase(Ease.InOutCubic)
                    .SetDelay(deathAnimateDuration)
            )
            .Append(deathCountText.transform.DOScale(Vector3.one, highlightDuration / 2).SetEase(Ease.InOutCubic))
            .Append(timeLabel.transform.DOScale(Vector3.one, appearDuration).SetEase(appearEase))
            .Append(
                timeText.transform.DOScale(Vector3.one, appearDuration)
                    .SetEase(appearEase)
                    .OnComplete(() => AnimateTime((float)time.TotalSeconds, numberAnimateDurationLong))
            )
            .Append(
                timeText.transform.DOScale(Vector3.one * highlightScale, highlightDuration / 2)
                    .SetEase(Ease.InOutCubic)
                    .SetDelay(numberAnimateDurationLong)
            )
            .Append(timeText.transform.DOScale(Vector3.one, highlightDuration / 2).SetEase(Ease.InOutCubic))
            .SetId(gameObject);
        
        bool newPB = personalBest == null || (TimeSpan)personalBest >= time;
        
        if (newPB) animationSequence.Append(newPBText.transform.DOScale(Vector3.one, appearDuration).SetEase(appearEase));
        else
        {
            animationSequence.Append(pbLabel.transform.DOScale(Vector3.one, appearDuration).SetEase(appearEase))
                .Append(pbText.transform.DOScale(Vector3.one, appearDuration).SetEase(appearEase));
        }
    }
    
    private void AnimateDeathCounter(int deaths, float duration) =>
        DOTween.To(GetCurrentDeathCountText, SetDeathCountText, deaths, duration)
            .SetEase(Ease.OutSine)
            .SetId(gameObject);
    
    private int GetCurrentDeathCountText() => int.Parse(deathCountText.text);
    private void SetDeathCountText(int value) => deathCountText.text = value.ToString();
    
    private void AnimateTime(float timeSeconds, float duration) =>
        DOTween.To(GetCurrentTimeText, SetTimeText, timeSeconds, duration)
            .SetEase(Ease.OutSine)
            .SetId(gameObject);
    
    private float GetCurrentTimeText() => (float)TimeSpan.Parse(timeText.text).TotalSeconds;
    private void SetTimeText(float value) => timeText.text = Utils.GetTimerString(value);
}