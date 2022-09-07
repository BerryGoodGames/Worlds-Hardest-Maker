using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    [HideInInspector] public KeyManager.KeyColor color;
    [HideInInspector] public Vector2 keyPosition;
    [HideInInspector] public bool pickedUp = false;

    private void Awake()
    {
        keyPosition = new(transform.position.x, transform.position.y);

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        int highestOrder = 0;
        foreach(Transform key in GameManager.Instance.KeyContainer.transform)
        {
            int order = key.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder;
            if (order > highestOrder) highestOrder = order;
        }
        renderer.sortingOrder = highestOrder + 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if collider is player
        if(collision.TryGetComponent(out PlayerController controller))
        {
            // check if player is of own client
            if (GameManager.Instance.Multiplayer && !controller.photonView.IsMine) return;
            
            // check if that player hasnt collected key yet
            if (!controller.keysCollected.Contains(gameObject))
            {
                PickUp(collision.gameObject);
            }
        }
    }

    private void PickUp(GameObject player)
    {
        PlayerController pcontroller = player.GetComponent<PlayerController>();
        pcontroller.keysCollected.Add(gameObject);

        // random rotation of pickup animation
        int randRotation = Random.Range(0, 2) * 90;
        transform.Rotate(0, 0, randRotation);

        // pickup animation
        Animator anim = transform.parent.GetComponent<Animator>();
        anim.SetBool("PickedUp", true);

        pickedUp = true;

        CheckAndUnlock(player);
    }

    public void CheckAndUnlock(GameObject player)
    {
        if (player.GetComponent<PlayerController>().KeysCollected(color))
        {
            string tagColor = "";
            if (color == KeyManager.KeyColor.RED) tagColor = "Red";
            else if (color == KeyManager.KeyColor.GREEN) tagColor = "Green";
            else if (color == KeyManager.KeyColor.BLUE) tagColor = "Blue";
            else if (color == KeyManager.KeyColor.YELLOW) tagColor = "Yellow";
            foreach (GameObject door in GameObject.FindGameObjectsWithTag(tagColor + "KeyDoorField"))
            {
                KeyDoorField controller = door.GetComponent<KeyDoorField>();
                controller.Lock(false);
            }
        }
    }
}
