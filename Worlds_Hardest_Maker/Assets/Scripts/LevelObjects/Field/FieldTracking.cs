using UnityEngine;

/// <summary>
///     Apply to every field which the player can stand on
/// </summary>
public class FieldTracking : MonoBehaviour
{
    private FieldController controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;

        if (!collider.CompareTag("Player")) return;

        PlayerController playerController = collider.GetComponent<PlayerController>();
        playerController.CurrentFields.Add(controller);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;

        if (!collider.CompareTag("Player")) return;

        PlayerController playerController = collider.GetComponent<PlayerController>();
        playerController.CurrentFields.Remove(controller);
    }

    private void Awake() => controller = GetComponent<FieldController>();
}