using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// checks if gameobjects colliders is hovered by mouse:
/// GetComponent<MouseOver>().Over
/// </summary>
public class MouseOver : MonoBehaviour
{
    public bool Over { get; set; } = false;
    private void Update()
    {
        Vector2 mousePos = MouseManager.GetMouseWorldPos();
        Collider2D[] colliders = gameObject.GetComponentsInChildren<Collider2D>();

        foreach(Collider2D collider in colliders)
        {
            if (collider.bounds.Contains(mousePos))
            {
                Over = true;
                return;
            }
        }
        Over = false;
    }
}
