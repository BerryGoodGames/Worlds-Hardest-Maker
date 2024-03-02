using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class HelpPopup : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform scrollContainer;
    [SerializeField] [PositiveValueOnly] private float scrollDuration;
    [Space] 
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform dotContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private Image dotPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Sprite dotFilledSprite;
    [SerializeField] [InitializationField] [MustBeAssigned] private Sprite dotOutlineSprite;

    private readonly Queue<RectTransform> movingLeft = new();
    private readonly Queue<RectTransform> movingRight = new();

    public int ScreenCount => scrollContainer.childCount;

    private int markedIndex = 0;
    
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
        
        SetDotFilled(markedIndex, false);
        markedIndex = Mod(markedIndex - 1, ScreenCount);
        SetDotFilled(markedIndex, true);
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
        
        SetDotFilled(markedIndex, false);
        markedIndex = (markedIndex + 1) % ScreenCount;
        SetDotFilled(markedIndex, true);
    }
    
    private void Awake()
    {
        Setup();
    }

    [ButtonMethod] [UsedImplicitly]
    public void Setup()
    {
        foreach (Transform screen in scrollContainer.transform)
        {
            NormalizeScreen((RectTransform)screen);
        }

#if UNITY_EDITOR
        for (int i = dotContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(dotContainer.GetChild(i).gameObject);
        }
#else
        dotContainer.DestroyChildren();
#endif
        
        for (int i = 0; i < scrollContainer.childCount; i++)
        {
            Sprite sprite = i == 0 ? dotFilledSprite : dotOutlineSprite;
            Image dot = Instantiate(dotPrefab, dotContainer);

            dot.sprite = sprite;
        }
    }

    private static void NormalizeScreen(RectTransform rt)
    {
        rt.anchorMin = new(0, rt.anchorMin.y);
        rt.anchorMax = new(1, rt.anchorMax.y);
    }

    private void SetDotFilled(int index, bool filled)
    {
        Image dot = dotContainer.GetChild(index).GetComponent<Image>();

        dot.sprite = filled ? dotFilledSprite : dotOutlineSprite;
    }
    
    private void OnDestroy() => DOTween.Kill(gameObject);
    
    private static int Mod(int x, int m) 
    {
        return (x%m + m)%m;
    }
}
