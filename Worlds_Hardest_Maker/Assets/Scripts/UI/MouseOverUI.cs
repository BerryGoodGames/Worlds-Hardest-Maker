using System;
using UnityEngine;

/// <summary>
///     Checks if ui element is hovered by mouse:
///     use moueOverUI.over
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class MouseOverUI : MonoBehaviour
{
    [HideInInspector] public bool Over;
    private RectTransform rt;
    private const bool updateSize = true;

    private Rect rtConverted;
    private float width;
    private float height;

    public Action OnHovered = () => { };
    public Action OnUnhovered = () => { };

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    public void UpdateSize()
    {
        rtConverted = UnitPixelUtils.RtToScreenSpace(rt);
        width = rtConverted.width;
        height = rtConverted.height;
    }

    private void Update()
    {
        if (updateSize) UpdateSize();
        if (Input.mousePosition.x > rt.position.x - width * 0.5f &&
            Input.mousePosition.x < rt.position.x + width * 0.5f &&
            Input.mousePosition.y > rt.position.y - height * 0.5f &&
            Input.mousePosition.y < rt.position.y + height * 0.5f)
        {
            if (!Over) OnHovered();

            Over = true;
        }
        else
        {
            if (Over) OnUnhovered();

            Over = false;
        }
    }
}