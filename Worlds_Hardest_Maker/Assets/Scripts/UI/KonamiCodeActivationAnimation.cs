using System;
using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class KonamiCodeActivationAnimation : MonoBehaviour
{
    [SerializeField] [Required] private AlphaTween blockerTween;
    [SerializeField] [Required] private AlphaTween alertTween;
    [SerializeField] [Required] private AlphaTween continueButtonTween;
    [FormerlySerializedAs("delay")]
    [Separator]
    [SerializeField] [PositiveValueOnly] private float soundDelay = 0.3f;
    [SerializeField] [PositiveValueOnly] private float alertDuration = 1f;
    [SerializeField] [PositiveValueOnly] private float alertStartScale = 4f;
    [SerializeField] [PositiveValueOnly] private float alertTargetScale = 1.5f;
    [SerializeField] [PositiveValueOnly] private float alertRise = 300f;
    [SerializeField] [PositiveValueOnly] private float waitTime = 3;
    
    public bool IsAnimationOnScreen { get; private set; }
    
    public void StartAnimation()
    {
        Setup();
        Animate();
    }
    
    private void Setup()
    {
        Time.timeScale = 0;
        
        alertTween.transform.DOScale(Vector3.one * alertStartScale, 0);
        ((RectTransform)alertTween.transform).DOAnchorPosY(-alertRise, 0);
        
        IsAnimationOnScreen = false;
    }
    
    private void Animate()
    {
        IsAnimationOnScreen = true;
        
        blockerTween.SetVisible(true)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => StartCoroutine(BlockerAppearComplete()));
        
        return;
        
        IEnumerator BlockerAppearComplete()
        {
            alertTween.SetVisible(true)
                .SetEase(Ease.InCirc);
            alertTween.transform.DOScale(Vector3.one * alertTargetScale, alertDuration)
                .SetEase(Ease.InCirc);
            
            ((RectTransform)alertTween.transform).DOAnchorPosY(300, alertDuration)
                .SetRelative()
                .SetEase(Ease.InCirc);
            
            yield return new WaitForSeconds(soundDelay);
            
            AudioManager.Instance.Play("ActivateKonamiCode");
            
            yield return new WaitForSeconds(waitTime);
            
            continueButtonTween.SetVisible(true);
        }
    }
    
    [UsedImplicitly]
    public void Continue()
    {
        Time.timeScale = 1;
        
        blockerTween.SetVisible(false);
        alertTween.SetVisible(false);
        continueButtonTween.SetVisible(false);
        
        IsAnimationOnScreen = false;
    }
}