using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script to detect player death event
/// attach to ball objects
/// </summary>
public class BallTriggerEvent : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if (collider.CompareTag("Player"))
        {
            PlayerController controller = collider.GetComponent<PlayerController>();

            if (GameManager.Instance.Multiplayer && !controller.photonView.IsMine) return;

            if (!controller.IsOnSafeField())
            {
                controller.DieNormal();
            }
        }
    }
}
