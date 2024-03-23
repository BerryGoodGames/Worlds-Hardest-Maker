using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MouseOverUIRect))]
public class PlayButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform top;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform button;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform chargebar;
    [Separator] 
    [SerializeField] [PositiveValueOnly] private float topHoverRotation;
    [SerializeField] [PositiveValueOnly] private float hoverRotationDuration;
    [Space]
    [SerializeField] [PositiveValueOnly] private float topChargeRotation;
    [SerializeField] [PositiveValueOnly] private float chargebarWidth;
    [SerializeField] [PositiveValueOnly] private float buttonChargeElevation;
    [SerializeField] [PositiveValueOnly] private float buttonChargeRotation;
    [SerializeField] [PositiveValueOnly] private float chargeDuration;
    [Space]
    [SerializeField] [PositiveValueOnly] private float topPlayRotation;
    [SerializeField] [PositiveValueOnly] private float playDuration;
    [Space] 
    [SerializeField] [PositiveValueOnly] private float playChargedDuration;
    
    private bool isCharging;
    private bool isPlaying;

    private bool shouldTogglePlay;

    private Vector2 idlePosition;
    
    private MouseOverUIRect mo;
    private bool mouseDown;
    private bool mouseUp;

    private void Start()
    {
        mo = GetComponent<MouseOverUIRect>();
        mo.OnHovered += OnHover;
        mo.OnUnhovered += OnUnhover;

        idlePosition = button.localPosition;
    }

    private void Update()
    {
        if (KeyBinds.GetKeyBindDown("Editor_PlayLevel") || mouseDown)
        {
            shouldTogglePlay = true;
            
            if (LevelSessionEditManager.Instance.Editing)
            {
                OnStartCharge();
            }
        }
        

        if ((KeyBinds.GetKeyBindUp("Editor_PlayLevel") || mouseUp) && shouldTogglePlay)
        {
            OnPlay();
            PlayManager.Instance.TogglePlay();
        }

        mouseDown = false;
        mouseUp = false;
    }

    #region Animations
    private void SetIdleAnim()
    {
        top.DOLocalRotate(Vector3.zero, hoverRotationDuration)
            .SetEase(Ease.InQuart);
    }

    private void SetHoverAnim()
    {
        Vector3 rotation = new(0, 0, topHoverRotation);

        top.DOLocalRotate(rotation, hoverRotationDuration)
            .SetEase(Ease.OutCubic);
    }

    private void StartChargeAnim()
    {
        Vector3 topRotation = new(0, 0, topHoverRotation);
        top.DOLocalRotate(topRotation, chargeDuration)
            .SetEase(Ease.InOutCubic);

        chargebar.DOSizeDelta(new(chargebarWidth, chargebar.rect.height), chargeDuration)
            .SetEase(Ease.Linear);

        button.DOLocalMoveY(buttonChargeElevation, chargeDuration)
            .SetRelative()
            .SetEase(Ease.InOutCubic);

        Vector3 buttonRotation = new(0, 0, buttonChargeRotation);
        button.DOLocalRotate(buttonRotation, chargeDuration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                shouldTogglePlay = false;
                OnPlayCharged();
                PlayManager.Instance.TogglePlay();
            });
    }

    private void PlayAnim()
    {
        top.DOKill();
        chargebar.DOKill();
        button.DOKill();
        
        top.DOLocalRotate(Vector3.forward * topPlayRotation, playDuration / 4)
            .SetEase(Ease.OutQuint);
        
        top.DOLocalRotate(Vector3.zero, playDuration * 3 / 4)
            .SetEase(Ease.OutBounce)
            .SetDelay(playDuration / 4);

        chargebar.DOSizeDelta(new(0, chargebar.rect.height), playDuration / 3)
            .SetEase(Ease.OutQuint);
        
        button.DOLocalMoveY(idlePosition.y, playDuration / 3)
            .SetEase(Ease.InQuint);

        button.DOLocalRotate(Vector3.zero, playDuration / 3)
            .SetEase(Ease.InQuint)
            .OnComplete(() => isPlaying = false);
    }

    private void PlayChargedAnim()
    {
        top.DOKill();
        chargebar.DOKill();
        button.DOKill();
        
        top.DOLocalRotate(Vector3.forward * topPlayRotation, playDuration / 4)
            .SetEase(Ease.OutQuint);
        
        top.DOLocalRotate(Vector3.zero, playDuration * 3 / 4)
            .SetEase(Ease.OutBounce)
            .SetDelay(playDuration / 4);

        chargebar.DOSizeDelta(new(0, chargebar.rect.height), playDuration / 3)
            .SetEase(Ease.OutQuint);
        
        button.DOLocalMoveY(idlePosition.y, playDuration / 3)
            .SetEase(Ease.InQuint);

        button.DOLocalRotate(Vector3.zero, playDuration / 3)
            .SetEase(Ease.InQuint)
            .OnComplete(() => isPlaying = false);
    }
    #endregion
    
    #region Events
    private void OnHover()
    {
        if (!isCharging && !isPlaying) SetHoverAnim();
    }

    private void OnUnhover()
    {
        if (!isCharging && !isPlaying) SetIdleAnim();
    }

    private void OnStartCharge()
    {
        if (!isPlaying)
        {
            StartChargeAnim();
            isCharging = true;
        }
    }

    private void OnPlay()
    {
        if (isCharging || LevelSessionEditManager.Instance.Playing)
        {
            PlayAnim();
            isPlaying = true;
            isCharging = false;
        }
    }

    private void OnPlayCharged()
    {
        if (isCharging)
        {
            PlayChargedAnim();
            isPlaying = true;
            isCharging = false;
        }
    }
    #endregion

    private void OnDestroy() => DOTween.Kill(gameObject);
    public void OnPointerDown(PointerEventData eventData) => mouseDown = true;
    public void OnPointerUp(PointerEventData eventData) => mouseUp = true;
}