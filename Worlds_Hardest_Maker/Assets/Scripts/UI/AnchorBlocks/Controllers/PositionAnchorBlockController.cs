using LuLib.Vector;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PositionAnchorBlockController : AnchorBlockController, IPointerEnterHandler, IPointerExitHandler
{
    [Separator("Position")] [InitializationField] [MustBeAssigned]
    public AnchorBlockPositionInputController PositionInput;

    public LineAnimator LineAnimator { get; set; }
    public LineRenderer Line => LineAnimator.LineRenderer;

    public (LineAnimator line1, LineAnimator line2) ArrowLines { get; set; }

    private GameObject blur;

    public new PositionAnchorBlock Block => (PositionAnchorBlock)base.Block;

    protected Vector2 GetPositionInput()
    {
        if (!float.TryParse(PositionInput.InputX.text, out float x) |
            !float.TryParse(PositionInput.InputY.text, out float y))
            Debug.LogWarning("Input in a Move Block was not a float");
        return new(x, y);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LineAnimator == null || Line == null) return;
        
        // calculate position etc.
        Vector2 delta = Line.GetPosition(1) - Line.GetPosition(0);
        Vector2 glowStart = Line.GetPosition(0) + ((Vector3)delta / 2);
        float glowLength = delta.magnitude;
        float glowRotation = delta.GetRotation();
        
        // create blur
        blur = Instantiate(PrefabManager.Instance.GlowPrefab, glowStart, Quaternion.Euler(0, 0, glowRotation), Line.transform);

        // configure sprite renderer settings
        SpriteRenderer spriteRenderer = blur.GetComponent<SpriteRenderer>();
        
        spriteRenderer.color = Line.startColor;

        const float glowWidth = 0.5f;
        spriteRenderer.size = new(glowWidth, glowLength + glowWidth);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(LineAnimator == null || Line == null || blur == null) return;

        Destroy(blur);
    }
}