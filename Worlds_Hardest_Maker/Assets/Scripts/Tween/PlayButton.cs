using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MouseOverUIRect))]
public class PlayButton : BarTween, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform top;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform button;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform bottom;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform chargebar;
    [Separator] 
    [SerializeField] [PositiveValueOnly] private float topHoverRotation;
    [SerializeField] [PositiveValueOnly] private float hoverRotationDuration;
    [Space]
    [SerializeField] [PositiveValueOnly] private float chargebarWidth;
    [SerializeField] [PositiveValueOnly] private float buttonChargeElevation;
    [SerializeField] [PositiveValueOnly] private float buttonChargeRotation;
    [SerializeField] [PositiveValueOnly] private float chargeDuration;
    [Space]
    [SerializeField] [PositiveValueOnly] private float topPlayRotation;
    [SerializeField] [PositiveValueOnly] private float bottomPlayRotation;
    [SerializeField] [PositiveValueOnly] private float playDuration;
    
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
        if (!ReferenceManager.Instance.Menu.activeSelf)
        {
            if ((KeyBinds.GetKeyBindDown("Editor_PlayLevel") || mouseDown) && !isCharging)
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
                PlayManager.Instance.TogglePlay(false);
            }
        }
        else
        {
            shouldTogglePlay = false;
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
                PlayManager.Instance.TogglePlay(true);
            });
    }

    private void PlayAnim()
    {
        top.DOKill();
        bottom.DOKill();
        chargebar.DOKill();
        button.DOKill();

        float durationAnticipation = playDuration * 0.4f;
        float restDuration = playDuration - durationAnticipation;
        float fallDuration = playDuration / 3;
        
        top.DOLocalRotate(Vector3.forward * topPlayRotation, durationAnticipation)
            .SetEase(Ease.OutQuint)
            .OnComplete(() => AudioManager.Instance.Play("PlayButtonClack"));

        top.DOLocalRotate(Vector3.zero, restDuration)
            .SetEase(Ease.OutBounce)
            .SetDelay(durationAnticipation);

        bottom.DOLocalRotate(Vector3.back * bottomPlayRotation, durationAnticipation)
            .SetEase(Ease.OutQuint);

        bottom.DOLocalRotate(Vector3.zero, restDuration / 3)
            .SetEase(Ease.OutCubic)
            .SetDelay(durationAnticipation);

        chargebar.DOSizeDelta(new(0, chargebar.rect.height), fallDuration)
            .SetEase(Ease.OutQuint);
        
        button.DOLocalMoveY(idlePosition.y, fallDuration)
            .SetEase(Ease.InQuint);

        button.DOLocalRotate(Vector3.zero, fallDuration)
            .SetEase(Ease.InQuint)
            .OnComplete(() => isPlaying = false);
    }

    private void PlayChargedAnim()
    {
        top.DOKill();
        bottom.DOKill();
        chargebar.DOKill();
        button.DOKill();
        
        float durationAnticipation = playDuration * 0.4f;
        float restDuration = playDuration - durationAnticipation;
        float fallDuration = playDuration / 3;
        
        top.DOLocalRotate(Vector3.forward * topPlayRotation, durationAnticipation)
            .SetEase(Ease.OutQuint)
            .OnComplete(() => AudioManager.Instance.Play("PlayButtonClack"));
        
        top.DOLocalRotate(Vector3.zero, restDuration)
            .SetEase(Ease.OutBounce)
            .SetDelay(playDuration / 4);

        bottom.DOLocalRotate(Vector3.back * bottomPlayRotation, durationAnticipation)
            .SetEase(Ease.OutQuint);

        bottom.DOLocalRotate(Vector3.zero, restDuration / 3)
            .SetEase(Ease.OutCubic)
            .SetDelay(durationAnticipation);

        chargebar.DOSizeDelta(new(0, chargebar.rect.height), fallDuration)
            .SetEase(Ease.OutQuint);
        
        button.DOLocalMoveY(idlePosition.y, fallDuration)
            .SetEase(Ease.InQuint);

        button.DOLocalRotate(Vector3.zero, fallDuration)
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