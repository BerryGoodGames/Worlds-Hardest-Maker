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
        foreach (AnchorPathLine line in Lines)
        {
            line.Blur.SetVisible(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (AnchorPathLine line in Lines)
        {
            line.Blur.SetVisible(false);
        }
    }

    // private void OnDestroy()
    // {
    //     foreach (AnchorPathLine line in Lines)
    //     {
    //         Destroy(line.gameObject);
    //     }
    // }

    private void Awake() => Lines = new();
}