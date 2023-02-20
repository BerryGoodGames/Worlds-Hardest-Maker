using UnityEngine;

/// <summary>
///     script to detect player death event
///     attach to ball objects
/// </summary>
public class BallTriggerEvent : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;

        if (!collider.CompareTag("Player")) return;

        PlayerController controller = collider.GetComponent<PlayerController>();

        if (MultiplayerManager.Instance.Multiplayer && !controller.PhotonView.IsMine) return;

        if (!controller.IsOnSafeField())
        {
            controller.DieNormal();
        }
    }
}