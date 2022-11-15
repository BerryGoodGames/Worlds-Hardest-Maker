using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCheckpoint : MonoBehaviour
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

    private static List<CCheckpoint> activatedCheckpoints = new();

    private CheckpointTween anim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = collision.gameObject;
        if (player.CompareTag("Player"))
        {
            // check if player wasnt on checkpoint before
            CPlayer controller = player.GetComponent<CPlayer>();

            bool alreadyOnField = controller.IsOnField(MField.FieldType.CHECKPOINT_FIELD);

            if ((!activated || reusableCheckpoints) && !alreadyOnField)
            {
                if (reusableCheckpoints) ClearCheckpoints();

                ChainActivate();

                controller.ActivateCheckpoint((int)transform.position.x, (int)transform.position.y);

                MAudio.Instance.Play("Checkpoint");
            }
        }
    }
    public void ChainActivate()
    {
        Activate();

        List<GameObject> neighbours = MField.GetNeighbours(gameObject);
        foreach(GameObject n in neighbours)
        {
            CCheckpoint checkpoint = n.GetComponent<CCheckpoint>();
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
        foreach (CCheckpoint controller in activatedCheckpoints)
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
