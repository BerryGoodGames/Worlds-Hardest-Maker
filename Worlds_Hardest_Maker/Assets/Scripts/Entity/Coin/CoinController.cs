using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    [HideInInspector] public Vector2 coinPosition;
    [HideInInspector] public bool pickedUp = false;
    private void Awake()
    {
        coinPosition = new(transform.position.x, transform.position.y);
    }
    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pickedUp)
        {
            // check if edgeCollider is player
            if (collision.TryGetComponent(out PlayerController controller))
            {
                // check if player is of own client
                if (GameManager.Instance.Multiplayer && !controller.photonView.IsMine) return;

                // check if that player hasnt picked coin up yet
                if (!controller.coinsCollected.Contains(gameObject))
                {
                    PickUp(collision.gameObject);

                    // check if player is in goal while collecting coin
                    if (controller.CoinsCollected())
                    {
                        foreach (GameObject field in controller.currentFields)
                        {
                            FieldManager.FieldType fieldType = (FieldManager.FieldType)FieldManager.GetFieldType(field);
                            if (fieldType == FieldManager.FieldType.GOAL_FIELD)
                            {
                                controller.Win();
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void PickUp(GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.coinsCollected.Add(gameObject);

        // coin counter, sfx, animation
        AudioManager.Instance.Play("Coin");

        Animator anim = transform.parent.GetComponent<Animator>();
        anim.SetBool("PickedUp", true);
        pickedUp = true;
    }
}
