using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public bool activated;
    private static bool reusableCheckpoints = true;
    public static bool ReusableCheckpoints {
        get => reusableCheckpoints;
        set
        {
            if(value) ResetCheckpoints();

            reusableCheckpoints = value;
        }
    }

    private static List<CheckpointController> activatedCheckpoints = new();

    private CheckpointTween anim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = collision.gameObject;
        if (!player.CompareTag("Player")) return;

        // check if player wasn't on checkpoint before
        PlayerController controller = player.GetComponent<PlayerController>();

        bool alreadyOnField = controller.IsOnField(FieldType.CHECKPOINT_FIELD);

        if ((activated && !reusableCheckpoints) || alreadyOnField) return;
        
        if (reusableCheckpoints) ResetCheckpoints();

        ChainActivate();

        Vector2 pos = transform.position;
        controller.ActivateCheckpoint(pos.x, pos.y);

        AudioManager.Instance.Play("Checkpoint");
    }
    public void ChainActivate()
    {
        Activate();

        List<GameObject> neighbors = FieldManager.GetNeighbors(gameObject);
        foreach(GameObject n in neighbors)
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

    private static void ResetCheckpoints()
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
