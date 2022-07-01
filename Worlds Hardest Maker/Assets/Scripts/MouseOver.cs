using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// checks if gameobjets colliders is hovered by mouse:
/// GetComponent<MouseOver>().over
/// </summary>
public class MouseOver : MonoBehaviour
{
    [HideInInspector] public bool over = false;
    private void Update()
    {
        Vector2 mousePos = GameManager.GetMouseWorldPos();
        Collider2D[] colliders = gameObject.GetComponentsInChildren<Collider2D>();

        foreach(Collider2D collider in colliders)
        {
            if (collider.bounds.Contains(mousePos))
            {
                over = true;
                return;
            }
        }
        over = false;
    }
}
