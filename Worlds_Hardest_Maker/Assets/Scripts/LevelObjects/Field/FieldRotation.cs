using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using WorldsHardestMaker.CopyPaste;
using WorldsHardestMaker.Selection;

public class FieldRotation : MonoBehaviour
{
    [SerializeField] [PositiveValueOnly] private float duration;
    [SerializeField] private Vector3 rotateAngle = new(0, 0, -90);
    [SerializeField] private float animationScale = 1.2f;
    [SerializeField] private bool disableCollision;
    [SerializeField] [ConditionalField(nameof(disableCollision))] [MustBeAssigned] private BoxCollider2D boxCollider;
    
    private FieldController controller;
    
    private Sequence scaleSequence;

    private ISelectionStateService selectionStateService;
    private ICopyPasteService copyPasteService;

    [Inject]
    private void Construct(ISelectionStateService selectionStateService, ICopyPasteService copyPasteService)
    {
        this.selectionStateService = selectionStateService;
        this.copyPasteService = copyPasteService;
    }
    
    private void Rotate()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        if (disableCollision) boxCollider.enabled = false;
        
        transform.DOComplete();
        scaleSequence?.Complete();
        
        transform.DORotate(rotateAngle, duration)
            .SetRelative()
            .SetEase(Ease.OutQuart)
            .OnComplete(
                () =>
                {
                    if (disableCollision) boxCollider.enabled = true;
                }
            );
        
        Vector3 originalScale = transform.localScale;
        
        scaleSequence = DOTween.Sequence();
        scaleSequence.Append(transform.DOScale(Vector3.one * animationScale, duration / 2).SetEase(Ease.OutCubic))
            .Append(transform.DOScale(originalScale, duration / 2).SetEase(Ease.InCubic));
    }
    
    private void OnMouseUpAsButton()
    {
        if (selectionStateService.IsSelecting || copyPasteService.IsPasting || LevelSessionEditManager.Instance.IsPlaying) return;
        
        if (LevelSessionEditManager.Instance.CurrentEditMode != controller.FieldMode) return;
        
        Rotate();
    }
    
    private void Awake() => controller = GetComponent<FieldController>();
}