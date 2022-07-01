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
    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        Rect rtConverted = GameManager.RtToScreenSpace(rt);
        float width = rtConverted.width;
        float height = rtConverted.height;
        if (Input.mousePosition.x > rt.position.x - width / 2 && Input.mousePosition.x < rt.position.x + width / 2 && Input.mousePosition.y > rt.position.y - height / 2 && Input.mousePosition.y < rt.position.y + height / 2) over = true;
        else over = false;
    }
}
