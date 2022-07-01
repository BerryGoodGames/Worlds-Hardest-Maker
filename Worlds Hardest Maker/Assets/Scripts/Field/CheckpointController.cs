using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if (collider.CompareTag("Player") && !activated)
        {
            ChainActivate();

            PlayerController controller = collider.GetComponent<PlayerController>();
            controller.ActivateCheckpoint((int)transform.position.x, (int)transform.position.y);
        }
    }
    public void ChainActivate()
    {
        activated = true;
        Animator anim = GetComponent<Animator>();
        anim.SetBool("Active", true);
        List<GameObject> neighbours = FieldManager.GetNeighbours(gameObject);
        foreach(GameObject n in neighbours)
        {
            CheckpointController checkpoint = n.GetComponent<CheckpointController>();
            if (checkpoint == null || checkpoint.activated) continue;
            checkpoint.ChainActivate();
        }
    }
}
