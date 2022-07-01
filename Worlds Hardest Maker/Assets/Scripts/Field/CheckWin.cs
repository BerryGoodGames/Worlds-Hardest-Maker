using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (GameManager.Instance.Playing && collider.gameObject.CompareTag("Player"))
        {
            GameObject player = PlayerManager.GetCurrentPlayer();
            PlayerController controller = player.GetComponent<PlayerController>();

            // check if every coin is collected
            if (controller.CoinsCollected())
            {
                controller.Win();
            }
        }
    }
}
