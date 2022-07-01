using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public bool pickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if (collider.CompareTag("Player") && !pickedUp)
        {
            PickUp();

            // check if player is in goal while collecting coin
            PlayerController controller = collider.GetComponent<PlayerController>();
            if(controller.CoinsCollected())
            {
                foreach(GameObject field in controller.currentFields)
                {
                    FieldManager.FieldType fieldType = (FieldManager.FieldType)FieldManager.GetFieldType(field);
                    if (fieldType == FieldManager.FieldType.GOAL_FIELD || fieldType == FieldManager.FieldType.START_AND_GOAL_FIELD)
                    {
                        controller.Win();
                        break;
                    }
                }
            }
        }
    }

    private void PickUp()
    {
        // coin counter, sfx, animation
        pickedUp = true;
        GameManager.Instance.CollectedCoins++;
        AudioManager.Instance.Play("Ding");

        Animator anim = transform.parent.GetComponent<Animator>();
        anim.SetBool("PickedUp", true);
    }
}
