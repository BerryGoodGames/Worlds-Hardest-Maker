using System;
using UnityEngine;

/// <summary>
///     checks if ui element is hovered by mouse:
///     GetComponent<MouseOverUI>().over
/// </summary>
public class MouseOverUI : MonoBehaviour
{
    [HideInInspector] public bool over;
    private RectTransform rt;
    private const bool updateSize = true;

    private Rect rtConverted;
    private float width;
    private float height;

    public Action onHovered = () => { };
    public Action onUnhovered = () => { };

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    public void UpdateSize()
    {
        rtConverted = GameManager.RtToScreenSpace(rt);
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
            if (!over)
            {
                onHovered();
            }

            over = true;
        }
        else
        {
            if (over)
            {
                onUnhovered();
            }

            over = false;
        }
    }
}