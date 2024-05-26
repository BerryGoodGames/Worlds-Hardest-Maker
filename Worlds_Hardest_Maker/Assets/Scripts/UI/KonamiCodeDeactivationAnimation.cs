using DG.Tweening;
using MyBox;
using UnityEngine;

public class KonamiCodeDeactivationAnimation : MonoBehaviour
{
    [SerializeField] private float targetDeltaY;
    [SerializeField] [PositiveValueOnly] private float duration;
    [SerializeField] [PositiveValueOnly] private float waitTime;
    
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
}
