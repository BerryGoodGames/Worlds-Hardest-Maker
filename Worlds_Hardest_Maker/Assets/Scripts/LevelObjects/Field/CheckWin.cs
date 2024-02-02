using UnityEngine;

/// <summary>
///     Attach to goal field
/// </summary>
public class CheckWin : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!LevelSessionEditManager.Instance.Playing ||
            !collider.gameObject.TryGetComponent(out PlayerController controller)) return;

        // check if every coin is collected
        if (controller.InDeathAnim || controller.Won || !CoinManager.Instance.AllCoinsCollected()) return;

        controller.Win();
    }
}