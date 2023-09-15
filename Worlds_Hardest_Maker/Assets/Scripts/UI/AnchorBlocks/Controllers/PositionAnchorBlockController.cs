using LuLib.Vector;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PositionAnchorBlockController : AnchorBlockController, IPointerEnterHandler, IPointerExitHandler
{
    [Separator("Position")] [InitializationField] [MustBeAssigned]
    public AnchorBlockPositionInputController PositionInput;

    [HideInInspector] public LineRenderer Line;


    public new PositionAnchorBlock Block => (PositionAnchorBlock)base.Block;

    private GameObject blur;
    protected Vector2 GetPositionInput()
    {
        if (!float.TryParse(PositionInput.InputX.text, out float x) |
            !float.TryParse(PositionInput.InputY.text, out float y))
            Debug.LogWarning("Input in a Move Block was not a float");
        return new(x, y);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Line == null) return;
        
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
        if(Line == null || blur == null) return;

        Destroy(blur);
    }
}