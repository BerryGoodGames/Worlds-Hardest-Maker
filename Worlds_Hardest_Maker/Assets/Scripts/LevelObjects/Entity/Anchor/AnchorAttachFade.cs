using DG.Tweening;
using MyBox;
using NaughtyAttributes;
using UnityEngine;

public class AnchorAttachFade : MonoBehaviour
{
    [SerializeField] [InitializationField] [Required] private Transform container;
    [Separator] [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float fadeInScalar = 1;
    [SerializeField] private float fadeOutScalar = 0.3f;
    
    private AnchorAttachment[] children;
    private float scalar;
    
    private void Start() => UpdateChildren();
    
    public void UpdateChildren() => children = container.GetComponentsInChildren<AnchorAttachment>();
    
    public void FadeOut() => FadeTo(fadeOutScalar, fadeDuration);
    
    public void FadeIn() => FadeTo(fadeInScalar, fadeDuration);
    
    private void UpdateOpacity()
    {
        foreach (AnchorAttachment child in children)
        {
            if (child == null) continue;
            if (child.AnchorAttachable == null) continue;
            
            SpriteRenderer sprite = child.AnchorAttachable.MainSprite;
            
            if (!child.CompareTag("Player")) sprite.DOKill();
            
            Color newColor = child.AnchorAttachable.MainSprite.color;
            newColor.a = scalar * child.Opacity;
            child.AnchorAttachable.MainSprite.color = newColor;
        }
    }
    
    private void SetOpacity(float scalar)
    {
        this.scalar = scalar;
        UpdateOpacity();
    }
    
    private void FadeTo(float scalar, float time)
    {
        UpdateChildren();
        DOTween.To(() => this.scalar, SetOpacity, scalar, time).SetUpdate(true);
    }
}