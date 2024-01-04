using DG.Tweening;
using Photon.Pun;
using UnityEngine;

public partial class AnchorManager
{
    public AnchorController SetAnchor(Vector2 position)
    {
        if (GetAnchor(position) != null) return null;

        GameObject anchor = MultiplayerManager.Instance.Multiplayer
            ? PhotonNetwork.Instantiate("Anchor", Vector2.zero, Quaternion.identity)
            : Instantiate(
                PrefabManager.Instance.Anchor, Vector2.zero, Quaternion.identity,
                ReferenceManager.Instance.AnchorContainer
            );

        AnchorController child = anchor.GetComponent<AnchorParentController>().Child;
        child.transform.position = position;

        // default blocks
        child.AppendBlock(new SetSpeedBlock(child, true, 5, SetSpeedBlock.Unit.Speed));
        child.AppendBlock(new SetRotationBlock(child, true, 1, SetRotationBlock.Unit.Iterations));
        child.AppendBlock(new SetDirectionBlock(child, true, true));
        child.AppendBlock(new SetEaseBlock(child, true, Ease.Linear));

        AnchorBallManager.Instance.AnchorBallListLayers.Add(child, new());

        return child;
    }

    public static void RemoveAnchor(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, 128);

        foreach (Collider2D hit in hits)
        {
            // check tag
            if (!hit.transform.parent.CompareTag("Anchor")) continue;

            RemoveAnchor(hit.GetComponent<AnchorController>());
            break;
        }
    }

    public static void RemoveAnchor(AnchorController anchor)
    {
        // deselect anchor first, if selected
        if (Instance.SelectedAnchor != null)
        {
            if (Instance.SelectedAnchor == anchor) Instance.DeselectAnchor();
        }

        AnchorBallManager.Instance.AnchorBallListLayers.Remove(anchor);

        // destroy anchor
        Destroy(anchor.transform.parent.gameObject);
    }

    public static AnchorController GetAnchor(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f);

        foreach (Collider2D hit in hits)
        {
            if (hit.transform.parent.CompareTag("Anchor")) return hit.gameObject.GetComponent<AnchorController>();
        }

        return null;
    }
}