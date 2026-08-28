using DG.Tweening;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;

public class KeyDoorFieldController : MonoBehaviour, IResettable
{
    [SerializeField] [InitializationField] [MustBeAssigned] private Animator animator;
    [SerializeField] [InitializationField] [MustBeAssigned] private Collider2D keyDoorCollider;
    [SerializeField] [InitializationField] [MustBeAssigned] private SpriteRenderer spriteRenderer;
    [SerializeField] [InitializationField] [MustBeAssigned] private FieldOutline fieldOutline;
    [SerializeField] [InitializationField] [PositiveValueOnly] private float fadeDuration;
    
    [Separator] [ReadOnly] [UsedImplicitly] public bool Unlocked;
    public KeyColor Color;
    
    private static readonly int unlockedString = Animator.StringToHash("Unlocked");
    
    [Inject] private EventBus eventBus;
    
    public void SetLocked(bool locked)
    {
        Unlocked = !locked;
        
        keyDoorCollider.enabled = locked;
        
        animator.SetBool(unlockedString, !locked);
    }
    
    private void Start() => ((IResettable)this).Subscribe(eventBus);
    
    public void ResetState() => SetLocked(true);
    
    public void FadeIn() => spriteRenderer.DOFade(1, fadeDuration).SetId(gameObject);
    
    public void FadeOut() => spriteRenderer.DOFade(0, fadeDuration).SetId(gameObject);
    
    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
        ((IResettable)this).Unsubscribe(eventBus);
    }
}