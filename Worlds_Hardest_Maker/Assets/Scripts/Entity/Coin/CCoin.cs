using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCoin : MonoBehaviour
{
    [HideInInspector] public Vector2 coinPosition;
    [HideInInspector] public bool pickedUp = false;
    private void Awake()
    {
        coinPosition = new(transform.position.x, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pickedUp)
        {
            // check if edgeCollider is player
            if (collision.TryGetComponent(out CPlayer controller))
            {
                // check if player is of own client
                if (MGame.Instance.Multiplayer && !controller.photonView.IsMine) return;

                // check if that player hasnt picked coin up yet
                if (!controller.coinsCollected.Contains(gameObject))
                {
                    PickUp(collision.gameObject);

                    // check if player is in goal while collecting coin
                    if (controller.CoinsCollected())
                    {
                        foreach (GameObject field in controller.currentFields)
                        {
                            MField.FieldType fieldType = (MField.FieldType)MField.GetFieldType(field);
                            if (fieldType == MField.FieldType.GOAL_FIELD)
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
        CPlayer controller = player.GetComponent<CPlayer>();
        controller.coinsCollected.Add(gameObject);

        // coin counter, sfx, animation
        MAudio.Instance.Play("Coin");

        Animator anim = transform.parent.GetComponent<Animator>();
        anim.SetBool("PickedUp", true);
        pickedUp = true;
    }
}
