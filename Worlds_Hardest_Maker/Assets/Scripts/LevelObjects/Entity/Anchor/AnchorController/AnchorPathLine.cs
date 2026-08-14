using DG.Tweening;
using LuLib.Vector;
using MyBox;
using UnityEngine;
using VContainer;

public class AnchorPathLine : MonoBehaviour
{
    public LineAnimator LineAnimator { get; set; }
    public LineRenderer LineRenderer => LineAnimator.LineRenderer;
    
    public (LineAnimator line1, LineAnimator line2) ArrowLines { get; set; }
    
    public AlphaTween Blur { get; private set; }

    private IDrawService drawService;
    
    [Inject]
    private void Construct(IDrawService drawService)
    {
        this.drawService = drawService;
    }
    
    public void AnimateEnd(Vector2 end)
    {
        if (LineRenderer == null)
        {
            Debug.LogWarning("LineRenderer is null when trying to render anchor lines");
            return;
        }
        
        LineAnimator.AnimatePoint(1, end, 0.05f, Ease.Linear);
        
        (Vector2 arrowVertex1, Vector2 arrowVertex2, Vector2 arrowCenter) =
            drawService.GetArrowHeadPoints(LineRenderer.GetPosition(0), end);
        
        ArrowLines.line1.AnimateAllPoints(new() { arrowCenter, arrowVertex1, }, 0.05f, Ease.Linear);
        ArrowLines.line2.AnimateAllPoints(new() { arrowCenter, arrowVertex2, }, 0.05f, Ease.Linear);
    }
    
    public void AnimateStart(Vector2 start)
    {
        LineAnimator.AnimatePoint(0, start, 0.05f, Ease.Linear);
        
        (Vector2 nextArrowVertex1, Vector2 nextArrowVertex2, Vector2 nextArrowCenter) =
            drawService.GetArrowHeadPoints(start, LineRenderer.GetPosition(1));
        
        ArrowLines.line1.AnimateAllPoints(new() { nextArrowCenter, nextArrowVertex1, }, 0.05f, Ease.Linear);
        ArrowLines.line2.AnimateAllPoints(new() { nextArrowCenter, nextArrowVertex2, }, 0.05f, Ease.Linear);
    }
    
    public void CreateArrowLine(Vector2 start, Vector2 end, bool dashed)
    {
        LineRenderer line = dashed
            ? drawService.DrawLine(start, end, transform)
            : drawService.DrawDashedLine(start, end, 0.2f, 0.2f, transform);
        
        LineAnimator = line.GetOrAddComponent<LineAnimator>();
    }
    
    public void CreateArrowHead(Vector2 start, Vector2 end)
    {
        (Vector2 arrowVertex1, Vector2 arrowVertex2, Vector2 arrowCenter) = drawService.GetArrowHeadPoints(start, end);
        
        (LineRenderer line1, LineRenderer line2) = (drawService.DrawLine(arrowCenter, arrowVertex1, transform),
            drawService.DrawLine(arrowCenter, arrowVertex2, transform));
        
        ArrowLines = (line1.GetOrAddComponent<LineAnimator>(), line2.GetOrAddComponent<LineAnimator>());
    }
    
    public void CreateBlur()
    {
        if (LineRenderer == null)
        {
            Debug.LogWarning("LineRenderer is null when trying to render anchor line blur");
            return;
        }
        
        // calculate position etc.
        Vector2 delta = LineRenderer.GetPosition(1) - LineRenderer.GetPosition(0);
        Vector2 glowStart = LineRenderer.GetPosition(0) + (Vector3)delta / 2;
        float glowLength = delta.magnitude;
        float glowRotation = delta.GetRotation();
        
        // create blur
        if (Blur == null)
        {
            Blur = Instantiate(
                PrefabManager.Instance.GlowPrefab, glowStart, Quaternion.Euler(0, 0, glowRotation),
                transform
            );
        }
        
        // configure sprite renderer settings
        SpriteRenderer spriteRenderer = Blur.GetComponent<SpriteRenderer>();
        
        spriteRenderer.color = LineRenderer.startColor.WithAlphaSetTo(0);
        
        const float GLOW_WIDTH = 0.5f;
        spriteRenderer.size = new(GLOW_WIDTH, glowLength + GLOW_WIDTH);
    }
}