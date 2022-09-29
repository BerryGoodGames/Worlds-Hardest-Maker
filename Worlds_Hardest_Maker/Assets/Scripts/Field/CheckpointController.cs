using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public bool activated = false;
    private static bool reusableCheckpoints = true;
    public static bool ReusableCheckpoints {
        get { return reusableCheckpoints; }
        set
        {
            if(value)
                ClearCheckpoints();

            reusableCheckpoints = value;
        }
    }

    private static List<CheckpointController> activatedCheckpoints = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if ((!activated || reusableCheckpoints) && collider.CompareTag("Player"))
        {
            if (reusableCheckpoints)
                ClearCheckpoints();

            ChainActivate();

            PlayerController controller = collider.GetComponent<PlayerController>();
            controller.ActivateCheckpoint((int)transform.position.x, (int)transform.position.y);
        }
    }
    public void ChainActivate()
    {
        Activate();
        List<GameObject> neighbours = FieldManager.GetNeighbours(gameObject);
        foreach(GameObject n in neighbours)
        {
            CheckpointController checkpoint = n.GetComponent<CheckpointController>();
            if (checkpoint == null || checkpoint.activated) continue;
            checkpoint.ChainActivate();
        }
    }

    public void Activate()
    {
        activated = true;
        Animator anim = GetComponent<Animator>();
        anim.SetBool("Active", true);
        activatedCheckpoints.Add(this);
    }

    public void Deactivate(bool remove = true)
    {
        activated = false;
        Animator anim = GetComponent<Animator>();
        anim.SetBool("Active", false);
        if(remove)
            activatedCheckpoints.Remove(this);

    }
    private static void ToggleReusableCheckpoints()
    {
        if (reusableCheckpoints) reusableCheckpoints = false;
        else
        {
            ClearCheckpoints();
            reusableCheckpoints = true;
        }
    }

    private static void ClearCheckpoints()
    {
        foreach (CheckpointController controller in activatedCheckpoints)
        {
            if (controller != null)
                controller.Deactivate(false);
        }

        activatedCheckpoints.Clear();
    }
}
