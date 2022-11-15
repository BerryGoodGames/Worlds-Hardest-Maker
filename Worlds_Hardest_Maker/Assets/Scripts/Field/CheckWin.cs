using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attach to (start-and-)goal field
/// </summary>
public class CheckWin : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (GameManager.Instance.Playing && collider.gameObject.TryGetComponent(out PlayerController controller))
        {
            // check if every coin is collected
            if (controller.CoinsCollected())
            {
                controller.Win();
            }
        }
    }
}
