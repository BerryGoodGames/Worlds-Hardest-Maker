using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    [HideInInspector] public bool pickedUp = false;
    [HideInInspector] public FieldManager.KeyDoorColor color;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if (collider.CompareTag("Player") && !pickedUp)
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        pickedUp = true;

        // random rotation of pickup animation
        int randRotation = Random.Range(0, 2) * 90;
        transform.Rotate(0, 0, randRotation);

        // pickup animation
        Animator anim = transform.parent.GetComponent<Animator>();
        anim.SetBool("PickedUp", true);

        PlayerController pcontroller = PlayerManager.GetCurrentPlayer().GetComponent<PlayerController>();
        if (pcontroller.KeysCollected(color))
        {
            string tagColor = "";
            if (color == FieldManager.KeyDoorColor.RED) tagColor = "Red";
            else if (color == FieldManager.KeyDoorColor.GREEN) tagColor = "Green";
            else if (color == FieldManager.KeyDoorColor.BLUE) tagColor = "Blue";
            else if (color == FieldManager.KeyDoorColor.YELLOW) tagColor = "Red";
            foreach (GameObject door in GameObject.FindGameObjectsWithTag(tagColor + "KeyDoorField"))
            {
                KeyDoorField controller = door.GetComponent<KeyDoorField>();
                controller.Lock(false);
            }
        }
    }
}
