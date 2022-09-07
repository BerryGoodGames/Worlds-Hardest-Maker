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
    private void OnMouseDrag()
    {
        if(!GameManager.Instance.Playing && Input.GetKey(GameManager.Instance.EntityMoveKey))
        {
            Vector2 newPos = MouseManager.Instance.MouseWorldPosMatrix;
            transform.position = newPos;
            if (TryGetComponent(out PathController controller))
            {
                controller.UpdateStartingPosition();
            }
        }
    }
}
