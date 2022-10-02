using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// makes entity drag and drop when shift is pressed
/// 
/// TODO: this is shit
/// </summary>
public class EntityDragDrop : MonoBehaviour
{
    [SerializeField] private bool halfGrid = false;
    public event Action onMove;
    private void OnMouseDrag()
    {
        if(!GameManager.Instance.Playing && Input.GetKey(GameManager.Instance.EntityMoveKey))
        {
            Vector2 newPos = halfGrid? MouseManager.Instance.MouseWorldPosGrid : MouseManager.Instance.MouseWorldPosMatrix;
            if (newPos != (Vector2)transform.position)
            {
                transform.position = newPos;

                if (onMove != null)
                    onMove();
            }
            if (TryGetComponent(out PathController controller))
            {
                controller.UpdateStartingPosition();
            }
        }
    }
}
