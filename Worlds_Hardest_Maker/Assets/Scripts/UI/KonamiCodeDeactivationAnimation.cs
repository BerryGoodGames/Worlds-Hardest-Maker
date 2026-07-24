using System;
using DG.Tweening;
using MyBox;
using UnityEngine;
using Zenject;

public class KonamiCodeDeactivationAnimation : MonoBehaviour
{
    [SerializeField] private float targetDeltaY;
    [SerializeField] [PositiveValueOnly] private float duration;
    [SerializeField] [PositiveValueOnly] private float waitTime;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
    }

    private void OnKonamiStateChanged(KonamiStateChangedEvent evt)
    {
        if (!evt.Active) StartAnimation();
    }

    public void StartAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        
        RectTransform rt = (RectTransform)transform;
        
        sequence.Append(
                rt.DOAnchorPosY(targetDeltaY, duration)
                    .SetRelative()
                    .SetEase(Ease.OutQuart)
            )
            .Append(
                rt.DOAnchorPosY(-targetDeltaY, duration)
                    .SetRelative()
                    .SetDelay(waitTime)
                    .SetEase(Ease.InQuart)
            );
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<KonamiStateChangedEvent>(OnKonamiStateChanged);
    }
}