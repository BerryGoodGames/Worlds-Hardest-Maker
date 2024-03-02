using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;

public class HelpPopup : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform scrollContainer;
    [SerializeField] [PositiveValueOnly] private float scrollDuration;

    private readonly Queue<RectTransform> movingLeft = new();
    private readonly Queue<RectTransform> movingRight = new();
    
    public void ScrollLeftButton()
    {
        if (movingLeft.Count > 0) return;
        
        // get last non-tweening screen
        RectTransform lastScreen = null;
        for (int i = 0; i < scrollContainer.childCount; i++)
        {
            RectTransform rt = (RectTransform)scrollContainer.GetChild(i);

            if (DOTween.IsTweening(rt)) continue;
            
            lastScreen = rt;
            break;
        }

        if (lastScreen == null) return;
        
        lastScreen.SetAsLastSibling();
        lastScreen.anchorMin = new(-1, lastScreen.anchorMin.y);
        lastScreen.anchorMax = new(0, lastScreen.anchorMax.y);
        
        lastScreen.DOAnchorMin(Vector2.right, scrollDuration)
            .SetEase(Ease.InOutSine)
            .SetRelative()
            .SetId(gameObject);
        
        lastScreen.DOAnchorMax(Vector2.right, scrollDuration)
            .SetEase(Ease.InOutSine)
            .SetRelative()
            .SetId(gameObject)
            .OnComplete(() => movingRight.Dequeue());
        
        movingRight.Enqueue(lastScreen);
    }

    public void ScrollRightButton()
    {
        if (movingRight.Count > 0) return;
        
        // get first non-tweening screen
        RectTransform firstScreen = null;
        for (int i = scrollContainer.childCount - 1; i >= 0; i--)
        {
            RectTransform rt = (RectTransform)scrollContainer.GetChild(i);

            if (DOTween.IsTweening(rt)) continue;
            
            firstScreen = rt;
            break;
        }

        if (firstScreen == null) return;
        
        firstScreen.DOAnchorMin(Vector2.left, scrollDuration)
            .SetEase(Ease.InOutSine)
            .SetRelative()
            .SetId(gameObject)
            .OnComplete(() =>
            {
                firstScreen.SetAsFirstSibling();
                firstScreen.anchorMin = new(0, firstScreen.anchorMin.y);
            });
        
        firstScreen.DOAnchorMax(Vector2.left, scrollDuration)
            .SetEase(Ease.InOutSine)
            .SetRelative()
            .SetId(gameObject)
            .OnComplete(
                () =>
                {
                    firstScreen.SetAsFirstSibling();
                    firstScreen.anchorMax = new(1, firstScreen.anchorMax.y);

                    movingLeft.Dequeue();
                });
        
        movingLeft.Enqueue(firstScreen);
    }
    
    private void Awake()
    {
        SetupScreens();
    }

    [ButtonMethod] [UsedImplicitly]
    public void SetupScreens()
    {
        foreach (Transform screen in scrollContainer.transform)
        {
            NormalizeScreen((RectTransform)screen);
        }
    }

    private static void NormalizeScreen(RectTransform rt)
    {
        rt.anchorMin = new(0, rt.anchorMin.y);
        rt.anchorMax = new(1, rt.anchorMax.y);
    }

    private void OnDestroy() => DOTween.Kill(gameObject);
}
