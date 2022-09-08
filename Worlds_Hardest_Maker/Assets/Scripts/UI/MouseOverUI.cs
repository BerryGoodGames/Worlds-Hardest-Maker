using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// checks if ui element is hovered by mouse:
/// GetComponent<MouseOverUI>().over
/// </summary>
public class MouseOverUI : MonoBehaviour
{
    [HideInInspector] public bool over = false;
    private RectTransform rt;
    private readonly bool updateSize = true;

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
        if(updateSize) UpdateSize();
        if (Input.mousePosition.x > rt.position.x - width / 2 &&
            Input.mousePosition.x < rt.position.x + width / 2 &&
            Input.mousePosition.y > rt.position.y - height / 2 &&
            Input.mousePosition.y < rt.position.y + height / 2)
        {
            if (!over)
            {
                onHovered();
            }

            over = true;
        }
        else 
        {
            if(over)
            {
                onUnhovered();
            }

            over = false;
        }
    }
}
