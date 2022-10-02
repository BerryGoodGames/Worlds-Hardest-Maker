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
    private void OnMouseDrag()
    {
        if(!GameManager.Instance.Playing && Input.GetKey(GameManager.Instance.EntityMoveKey))
        {
            Vector2 newPos = halfGrid? MouseManager.Instance.MouseWorldPosGrid : MouseManager.Instance.MouseWorldPosMatrix;
            transform.position = newPos;
            if (TryGetComponent(out PathController controller))
            {
                controller.UpdateStartingPosition();
            }
        }
    }
}
