using DG.Tweening;
using MyBox;
using NaughtyAttributes;
using UnityEngine;

public class KeyDoorFieldController : MonoBehaviour, IResettable
{
    [SerializeField] [InitializationField] [Required] private Animator animator;
    [SerializeField] [InitializationField] [Required] private Collider2D keyDoorCollider;
    [SerializeField] [InitializationField] [Required] private SpriteRenderer spriteRenderer;
    [SerializeField] [InitializationField] [Required] private FieldOutline fieldOutline;
    [SerializeField] [InitializationField] [PositiveValueOnly] private float fadeDuration;
    
    [Separator] [MyBox.ReadOnly] public bool Unlocked;
    public KeyColor Color;
    
    private static readonly int unlockedString = Animator.StringToHash("Unlocked");
    
    public void SetLocked(bool locked)
    {
        Unlocked = !locked;
        
        keyDoorCollider.enabled = locked;
        
        animator.SetBool(unlockedString, !locked);
    }
    
    private void Start() => ((IResettable)this).Subscribe();
    
    public void ResetState() => SetLocked(true);
    
    public void FadeIn() => spriteRenderer.DOFade(1, fadeDuration).SetId(gameObject);
    
    public void FadeOut() => spriteRenderer.DOFade(0, fadeDuration).SetId(gameObject);
    
    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
        ((IResettable)this).Unsubscribe();
    }
}