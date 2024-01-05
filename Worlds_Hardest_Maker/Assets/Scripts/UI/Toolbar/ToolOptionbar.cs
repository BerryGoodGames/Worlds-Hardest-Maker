using System;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteAlways]
public class ToolOptionbar : MonoBehaviour
{
    [ReadOnly] public Tool Root;

    [Separator] [SerializeField] private RectTransform toolPrefab;

    [FormerlySerializedAs("HoveringHitbox")] [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform hoveringHitbox;
    [SerializeField] [InitializationField] [MustBeAssigned] private VerticalLayoutGroup optionsLayoutGroup;
    [SerializeField] [InitializationField] [MustBeAssigned] private AlphaTween anim;

    [Separator] [PositiveValueOnly] [SerializeField] private float width;

    private int ToolCount => optionsLayoutGroup.transform.childCount;

    [ButtonMethod]
    public void UpdateHeight()
    {
        RectTransform rt = (RectTransform)transform;

        // set (width and) height of tool optionbar to fit every option
        Rect toolRect = toolPrefab.rect;

        float toolMargin = (width - toolRect.width) / 2;

        Vector2 size = ToolCount == 0
            ? width * Vector2.one
            : new(width, toolRect.height * ToolCount + 2 * toolMargin);

        rt.sizeDelta = size;

        // set hovering hitbox width and height
        const float arrowHeight = 35;
        hoveringHitbox.offsetMax = new Vector2(hoveringHitbox.offsetMax.x, arrowHeight);

        // set top offset in vertical layout group
        optionsLayoutGroup.padding.top = (int)toolMargin;
    }

    private void Awake() => UpdateHeight();

    private void Start()
    {
        if (!transform.parent.TryGetComponent(out Root))
        {
            throw new Exception(
                "This tool optionbar does not have a tool as root\ntool optionbar is expected to be direct child of a tool"
            );
        }

        // teleport optionbar up when invisible (dont care)
        RectTransform rt = (RectTransform)transform;
        Vector2 visiblePosition = rt.anchoredPosition;
        Vector2 invisiblePosition = visiblePosition + Vector2.up * 1000;

        anim.OnIsInvisible += () => rt.anchoredPosition = invisiblePosition;
        anim.OnSetVisible += () => rt.anchoredPosition = visiblePosition;
    }
}