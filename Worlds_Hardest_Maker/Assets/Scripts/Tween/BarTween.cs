using DG.Tweening;
using UnityEngine;
using VContainer;

/// <summary>
///     General tweening script for UI at the top or bottom of the screen
///     <para>Tweens UI Element offscreen when playing (playingY) and onscreen when editing (editingY) with SetPlay</para>
/// </summary>
public class BarTween : MonoBehaviour
{
    [SerializeField] private float visibleY;
    [SerializeField] private float invisibleY;
    [SerializeField] private bool isVisibleOnlyOnEdit = true;
    [Space] [SerializeField] private float appearDuration;
    [SerializeField] private float disappearDuration;
    [Space] [SerializeField] private Ease easeAppear;
    [SerializeField] private Ease easeDisappear;
    [SerializeField] private AnimationCurve easeAppearCurve;
    [SerializeField] private AnimationCurve easeDisappearCurve;
    
    // when playing is null, it means the object is in some other state and can switch back to edit or play anytime
    private bool? playing;
    
    private RectTransform rt;
    
    private Tween tween;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<TogglePlayEditEvent>(OnTogglePlayEdit);
    }
    
    private void OnTogglePlayEdit(TogglePlayEditEvent evt) => SetPlay(LevelSessionEditManager.Instance.Playing);

    public void SetPlay(bool play)
    {
        if ((playing == null && !play) || (playing != null && (bool)playing && !play))
        {
            // the frame unplayed -> editmode
            if (isVisibleOnlyOnEdit) TweenVis();
            else TweenInvis();
        }
        
        if ((playing == null && play) || (playing != null && !(bool)playing && play))
        {
            // the frame played -> playmode
            if (!isVisibleOnlyOnEdit) TweenVis();
            else TweenInvis();
        }
        
        playing = play;
    }
    
    public void TweenToY(float y, bool isResultVisibleState, bool nullPlayState = true)
    {
        tween?.Kill();
        
        Ease ease = isResultVisibleState ? easeAppear : easeDisappear;
        AnimationCurve curve = isResultVisibleState ? easeAppearCurve : easeDisappearCurve;
        float duration = isResultVisibleState ? appearDuration : disappearDuration;
        
        tween = rt.DOAnchorPosY(y, duration).SetId(gameObject);
        if (curve.length > 1) tween.SetEase(curve);
        else tween.SetEase(ease);
        
        if (nullPlayState) playing = null;
        else playing = !isResultVisibleState;
    }
    
    public void TweenInvis() => TweenToY(invisibleY, false, false);
    
    public void TweenVis() => TweenToY(visibleY, true, false);
    
    private void Start()
    {
        rt = (RectTransform)transform;
        
        if (LevelSessionEditManager.Instance.Playing) rt.anchoredPosition = new(rt.anchoredPosition.x, isVisibleOnlyOnEdit ? invisibleY : visibleY);
        else rt.anchoredPosition = new(rt.anchoredPosition.x, !isVisibleOnlyOnEdit ? invisibleY : visibleY);
    }
    
    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
        eventBus.Unsubscribe<TogglePlayEditEvent>(OnTogglePlayEdit);
    }
}