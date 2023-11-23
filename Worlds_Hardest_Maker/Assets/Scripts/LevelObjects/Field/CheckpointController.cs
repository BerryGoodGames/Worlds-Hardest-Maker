using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public bool Activated;
    private static bool reusableCheckpoints = true;

    public static bool ReusableCheckpoints
    {
        get => reusableCheckpoints;
        set
        {
            if (value) ResetCheckpoints();

            reusableCheckpoints = value;
        }
    }

    private static readonly List<CheckpointController> activatedCheckpoints = new();

    private CheckpointTween anim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = collision.gameObject;
        if (!player.CompareTag("Player")) return;

        // check if player wasn't on checkpoint before
        PlayerController controller = player.GetComponent<PlayerController>();

        bool alreadyOnField = controller.IsOnField(FieldType.Checkpoint);

        if ((Activated && !reusableCheckpoints) || alreadyOnField) return;

        if (reusableCheckpoints) ResetCheckpoints();

        ChainActivate();

        Vector2 pos = transform.position;
        controller.ActivateCheckpoint(pos);

        AudioManager.Instance.Play("Checkpoint");
    }

    public void ChainActivate()
    {
        Activate();

        List<FieldController> neighbors = FieldManager.GetNeighbors(gameObject);
        foreach (FieldController n in neighbors)
        {
            if (!n.TryGetComponent(out CheckpointController checkpoint) || checkpoint.Activated) continue;

            checkpoint.ChainActivate();
        }
    }

    public void Activate()
    {
        Activated = true;

        activatedCheckpoints.Add(this);

        anim.Activate(reusableCheckpoints);
    }

    public void Deactivate(bool remove = true)
    {
        Activated = false;

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

    private void Start() => anim = GetComponent<CheckpointTween>();
}