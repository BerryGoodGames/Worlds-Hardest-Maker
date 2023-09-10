using UnityEngine;

/// <summary>
///     Attach to goal field
/// </summary>
public class CheckWin : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!EditModeManager.Instance.Playing ||
            !collider.gameObject.TryGetComponent(out PlayerController controller)) return;

        // check if every coin is collected
        if (controller.InDeathAnim || controller.Won || !controller.AllCoinsCollected()) return;

        controller.Win();
    }
}