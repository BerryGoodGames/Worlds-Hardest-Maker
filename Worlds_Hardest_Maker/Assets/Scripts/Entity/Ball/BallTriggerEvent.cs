using UnityEngine;

/// <summary>
///     Detects player death event
///     <para>Attach to ball objects</para>
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