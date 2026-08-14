using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

[RequireComponent(typeof(MouseOverUIRect))]
public class PlayButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform top;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform button;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform bottom;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform chargebar;
    [Separator] [SerializeField] [PositiveValueOnly] private float topHoverRotation;
    [SerializeField] [PositiveValueOnly] private float hoverRotationDuration;
    [Space] [SerializeField] [PositiveValueOnly] private float chargebarWidth;
    [SerializeField] [PositiveValueOnly] private float buttonChargeElevation;
    [SerializeField] [PositiveValueOnly] private float buttonChargeRotation;
    [SerializeField] [PositiveValueOnly] private float chargeDuration;
    [Space] [SerializeField] [PositiveValueOnly] private float topPlayRotation;
    [SerializeField] [PositiveValueOnly] private float bottomPlayRotation;
    [SerializeField] [PositiveValueOnly] private float playDuration;
    
    private bool isCharging;
    private bool isPlaying;
    
    private bool shouldTogglePlay;
    
    private Vector2 idlePosition;
    
    private MouseOverUIRect mo;
    private bool mouseDown;
    private bool mouseUp;
    
    private IAudioService audioService;
    
    [Inject]
    private void Construct(IAudioService audioService)
    {
        this.audioService = audioService;
    }

    private void Start()
    {
        mo = GetComponent<MouseOverUIRect>();
        mo.OnHovered += OnHover;
        mo.OnUnhovered += OnUnhover;
        
        idlePosition = button.localPosition;
    }
    
    private void Update()
    {
        if (!ReferenceManager.Instance.Menu.activeSelf)
        {
            if ((KeyBinds.GetKeyBindDown("Editor_PlayLevel") || mouseDown) && !isCharging)
            {
                shouldTogglePlay = true;
                
                if (LevelSessionEditManager.Instance.Editing) OnStartCharge();
            }
            
            if ((KeyBinds.GetKeyBindUp("Editor_PlayLevel") || mouseUp) && shouldTogglePlay)
            {
                OnPlay();
                PlayManager.Instance.TogglePlay(false);
            }
        }
        else shouldTogglePlay = false;
        
        mouseDown = false;
        mouseUp = false;
    }
    
    #region Animations
    
    private void SetIdleAnim() =>
        top.DOLocalRotate(Vector3.zero, hoverRotationDuration)
            .SetEase(Ease.InQuart)
            .SetUpdate(true);
    
    private void SetHoverAnim()
    {
        Vector3 rotation = new(0, 0, topHoverRotation);
        
        top.DOLocalRotate(rotation, hoverRotationDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }
    
    private void StartChargeAnim()
    {
        Vector3 topRotation = new(0, 0, topHoverRotation);
        top.DOLocalRotate(topRotation, chargeDuration)
            .SetEase(Ease.InOutCubic)
            .SetUpdate(true);
        
        chargebar.DOSizeDelta(new(chargebarWidth, chargebar.rect.height), chargeDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true);
        
        button.DOLocalMoveY(buttonChargeElevation, chargeDuration)
            .SetRelative()
            .SetEase(Ease.InOutCubic)
            .SetUpdate(true);
        
        Vector3 buttonRotation = new(0, 0, buttonChargeRotation);
        button.DOLocalRotate(buttonRotation, chargeDuration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(
                () =>
                {
                    shouldTogglePlay = false;
                    OnPlayCharged();
                    PlayManager.Instance.TogglePlay(true);
                }
            )
            .SetUpdate(true);
    }
    
    private void PlayAnim()
    {
        top.DOKill();
        bottom.DOKill();
        chargebar.DOKill();
        button.DOKill();
        
        float durationAnticipation = playDuration * 0.4f;
        float restDuration = (playDuration - durationAnticipation) / 3;
        float fallDuration = playDuration / 3;
        
        top.DOLocalRotate(Vector3.forward * topPlayRotation, durationAnticipation)
            .SetEase(Ease.OutSine)
            .OnComplete(() => audioService.Play("PlayButtonClack"))
            .SetUpdate(true);
        
        top.DOLocalRotate(Vector3.zero, restDuration)
            .SetEase(Ease.InCubic)
            .SetDelay(durationAnticipation)
            .SetUpdate(true);
        
        bottom.DOLocalRotate(Vector3.back * bottomPlayRotation, durationAnticipation)
            .SetEase(Ease.OutSine)
            .SetUpdate(true);
        
        bottom.DOLocalRotate(Vector3.zero, restDuration * 1.1f)
            .SetEase(Ease.OutCubic)
            .SetDelay(durationAnticipation)
            .SetUpdate(true);
        
        chargebar.DOSizeDelta(new(0, chargebar.rect.height), fallDuration)
            .SetEase(Ease.OutQuint)
            .SetUpdate(true);
        
        button.DOLocalMoveY(idlePosition.y, fallDuration)
            .SetEase(Ease.InQuint)
            .SetUpdate(true);
        
        button.DOLocalRotate(Vector3.zero, fallDuration)
            .SetEase(Ease.InQuint)
            .OnComplete(() => isPlaying = false)
            .SetUpdate(true);
    }
    
    private void PlayChargedAnim()
    {
        top.DOKill();
        bottom.DOKill();
        chargebar.DOKill();
        button.DOKill();
        
        float durationAnticipation = playDuration * 0.4f;
        float restDuration = (playDuration - durationAnticipation) / 3;
        float fallDuration = playDuration / 3;
        
        top.DOLocalRotate(Vector3.forward * topPlayRotation, durationAnticipation)
            .SetEase(Ease.OutSine)
            .OnComplete(() => audioService.Play("PlayButtonClack"))
            .SetUpdate(true);
        
        top.DOLocalRotate(Vector3.zero, restDuration)
            .SetEase(Ease.InCubic)
            .SetDelay(playDuration / 4)
            .SetUpdate(true);
        
        bottom.DOLocalRotate(Vector3.back * bottomPlayRotation, durationAnticipation)
            .SetEase(Ease.OutSine)
            .SetUpdate(true);
        
        bottom.DOLocalRotate(Vector3.zero, restDuration * 1.1f)
            .SetEase(Ease.OutCubic)
            .SetDelay(durationAnticipation)
            .SetUpdate(true);
        
        chargebar.DOSizeDelta(new(0, chargebar.rect.height), fallDuration)
            .SetEase(Ease.OutQuint)
            .SetUpdate(true);
        
        button.DOLocalMoveY(idlePosition.y, fallDuration)
            .SetEase(Ease.InQuint)
            .SetUpdate(true);
        
        button.DOLocalRotate(Vector3.zero, fallDuration)
            .SetEase(Ease.InQuint)
            .OnComplete(() => isPlaying = false)
            .SetUpdate(true);
    }
    
    #endregion
    
    private bool IsBlockedByOtherUI()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null) return false;
        
        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition,
        };
        
        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, results);
        
        if (results.Count == 0) return false;
        
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == gameObject || result.gameObject.transform.IsChildOf(transform))
            {
                return false;
            }
            
            if (result.gameObject.activeInHierarchy)
            {
                return true;
            }
        }
        
        return false;
    }
    
    #region Events
    
    private void OnHover()
    {
        if (!isCharging && !isPlaying && !IsBlockedByOtherUI()) SetHoverAnim();
    }
    
    private void OnUnhover()
    {
        if (!isCharging && !isPlaying) SetIdleAnim();
    }
    
    private void OnStartCharge()
    {
        if (isPlaying) return;
        
        StartChargeAnim();
        isCharging = true;
    }
    
    private void OnPlay()
    {
        if (!isCharging && !LevelSessionEditManager.Instance.Playing) return;
        
        PlayAnim();
        isPlaying = true;
        isCharging = false;
    }
    
    private void OnPlayCharged()
    {
        if (!isCharging) return;
        
        PlayChargedAnim();
        isPlaying = true;
        isCharging = false;
    }
    
    #endregion
    
    private void OnDestroy() => DOTween.Kill(gameObject);
    public void OnPointerDown(PointerEventData eventData) => mouseDown = true;
    public void OnPointerUp(PointerEventData eventData) => mouseUp = true;
}