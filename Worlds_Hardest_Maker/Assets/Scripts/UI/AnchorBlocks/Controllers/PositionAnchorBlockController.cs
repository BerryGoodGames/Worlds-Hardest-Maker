using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PositionAnchorBlockController : AnchorBlockController, IPointerEnterHandler, IPointerExitHandler
{
    [Separator("Position")] [InitializationField] [MustBeAssigned]
    public AnchorBlockPositionInputController PositionInput;

    public List<AnchorPathLine> Lines { get; set; }

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
        SetBlurVisible(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetBlurVisible(false);
    }

    public void SetBlurVisible(bool visible)
    {
        foreach (AnchorPathLine line in Lines)
        {
            line.Blur.SetVisible(visible);
        }
    }

    private void Awake() => Lines = new();
}