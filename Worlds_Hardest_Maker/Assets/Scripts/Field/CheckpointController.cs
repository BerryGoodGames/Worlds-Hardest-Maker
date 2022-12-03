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
            if(value) ClearCheckpoints();

            reusableCheckpoints = value;
        }
    }

    private static List<CheckpointController> activatedCheckpoints = new();

    private CheckpointTween anim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = collision.gameObject;
        if (player.CompareTag("Player"))
        {
            // check if player wasnt on checkpoint before
            PlayerController controller = player.GetComponent<PlayerController>();

            bool alreadyOnField = controller.IsOnField(FieldType.CHECKPOINT_FIELD);

            if ((!activated || reusableCheckpoints) && !alreadyOnField)
            {
                if (reusableCheckpoints) ClearCheckpoints();

                ChainActivate();

                controller.ActivateCheckpoint((int)transform.position.x, (int)transform.position.y);

                AudioManager.Instance.Play("Checkpoint");
            }
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

        activatedCheckpoints.Add(this);

        anim.Activate(reusableCheckpoints);
    }

    public void Deactivate(bool remove = true)
    {
        activated = false;

        if (remove) activatedCheckpoints.Remove(this);

        anim.Deactivate();
    }

    private static void ClearCheckpoints()
    {
        // deactivate every checkpoint
        foreach (CheckpointController controller in activatedCheckpoints)
        {
            if (controller != null) controller.Deactivate(false);
        }

        activatedCheckpoints.Clear();
    }

    private void Start()
    {
        anim = GetComponent<CheckpointTween>();
    }
}
