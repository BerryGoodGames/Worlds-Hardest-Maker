using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Apply this to every field which the player can stand on
/// </summary>
public class FieldTracking : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if (collider.CompareTag("Player"))
        {
            CPlayer controller = collider.GetComponent<CPlayer>();
            controller.currentFields.Add(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if (collider.CompareTag("Player"))
        {
            CPlayer controller = collider.GetComponent<CPlayer>();
            controller.currentFields.Remove(gameObject);
        }
    }

}
