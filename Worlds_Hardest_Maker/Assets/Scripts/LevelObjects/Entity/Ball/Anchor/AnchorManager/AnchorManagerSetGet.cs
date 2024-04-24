using DG.Tweening;
using UnityEngine;

public partial class AnchorManager : IManager<AnchorController>
{
    public Transform DefaultContainer => ReferenceManager.Instance.AnchorContainer;
    
    public AnchorController SetInSheet(ManagerParameters args)
    {
        if (GetInSheet(args.Position, args.Sheet) != null) return null;
        
        AnchorController anchor = InstantiateInSheet(args);
        anchor.transform.position = args.Position;

        // default blocks
        anchor.AppendBlock(new SetSpeedBlock(anchor, true, 5, SetSpeedBlock.Unit.Speed));
        anchor.AppendBlock(new SetRotationBlock(anchor, true, 1, SetRotationBlock.Unit.Iterations));
        anchor.AppendBlock(new SetDirectionBlock(anchor, true, true));
        anchor.AppendBlock(new SetEaseBlock(anchor, true, Ease.Linear));

        AnchorBallManager.Instance.AnchorBallListSheets.Add(anchor, new());

        return anchor;
    }

    public AnchorController GetInSheet(Vector2 position, AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f);

        foreach (Collider2D hit in hits)
        {
            if (hit.transform.parent.CompareTag("Anchor")) return hit.gameObject.GetComponent<AnchorController>();
        }

        return null;
    }

    public static void Remove(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, 128);

        foreach (Collider2D hit in hits)
        {
            // check tag
            if (!hit.transform.parent.CompareTag("Anchor")) continue;

            Remove(hit.GetComponent<AnchorController>());
            break;
        }
    }

    public static void Remove(AnchorController anchor)
    {
        // deselect anchor first, if selected
        if (Instance.SelectedAnchor != null)
            if (Instance.SelectedAnchor == anchor)
                Instance.DeselectAnchor();

        AnchorBallManager.Instance.AnchorBallListSheets.Remove(anchor);

        // destroy anchor
        Destroy(anchor.transform.parent.gameObject);

        AudioManager.Instance.Play(PlaceManager.Instance.GetSfx(EditModeManager.Delete));
    }

    public AnchorController InstantiateInSheet(ManagerParameters args)
    {
        return Instantiate(
            PrefabManager.Instance.Anchor, Vector2.zero, Quaternion.identity,
            DefaultContainer
        ).Child;
    }
    public bool CorrespondsToEditMode(EditMode compare) => compare == EditModeManager.Anchor;
}