using System;
using MyBox;
using UnityEngine;

[ExecuteAlways]
public class ToolOptionbar : MonoBehaviour
{
    [ReadOnly] public Tool Root;
    
    [Separator] [SerializeField] private RectTransform toolPrefab;
    
    public RectTransform HoveringHitbox;
    public RectTransform Options;

    private int ToolCount => Options.childCount;

    [ButtonMethod]
    public void UpdateHeight()
    {
        RectTransform rt = (RectTransform)transform;
        
        // set (width and) height of tool optionbar to fit every option
        Rect toolRect = toolPrefab.rect;
        
        Vector2 size = ToolCount == 0 
            ? toolRect.width * Vector2.one 
            : new(toolRect.width, toolRect.height * ToolCount);
        
        rt.sizeDelta = size;
        
        // set hovering hitbox width and height
        const float arrowHeight = 35;
        HoveringHitbox.offsetMax = new Vector2(HoveringHitbox.offsetMax.x, arrowHeight);
    }
    
    private void Awake() => UpdateHeight();

    private void Start()
    {
        if (!transform.parent.TryGetComponent(out Root))
        {
            throw new Exception(
                "This tool optionbar does not have a tool as root\ntool optionbar is expected to be direct child of a tool");
        }
    }
}