using DG.Tweening;
using Photon.Pun;
using UnityEngine;

public partial class AnchorManager
{
    public AnchorController SetAnchor(float mx, float my) => SetAnchor(new(mx, my));

    public AnchorController SetAnchor(Vector2 pos)
    {
        if (GetAnchor(pos) != null) return null;

        GameObject anchor = MultiplayerManager.Instance.Multiplayer
            ? PhotonNetwork.Instantiate("Anchor", Vector2.zero, Quaternion.identity)
            : Instantiate(PrefabManager.Instance.Anchor, Vector2.zero, Quaternion.identity,
                ReferenceManager.Instance.AnchorContainer);

        AnchorController child = anchor.GetComponent<AnchorParentController>().Child;
        child.transform.position = pos;

        // default blocks
        child.AppendBlock(new SetSpeedBlock(child, true, 5, SetSpeedBlock.Unit.Speed));
        child.AppendBlock(new SetRotationBlock(child, true, 1, SetRotationBlock.Unit.Iterations));
        child.AppendBlock(new SetDirectionBlock(child, true, true));
        child.AppendBlock(new SetEaseBlock(child, true, Ease.Linear));

        return child;
    }

    [PunRPC]
    public void RemoveAnchor(float mx, float my) => RemoveAnchor(new(mx, my));

    public static void RemoveAnchor(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.01f, 128);

        foreach (Collider2D hit in hits)
        {
            // check tag
            if (!hit.transform.parent.CompareTag("Anchor")) continue;

            // deselect anchor first, if selected
            if (Instance.SelectedAnchor != null)
            {
                AnchorController anchor = hit.GetComponent<AnchorController>();
                if (Instance.SelectedAnchor.Equals(anchor)) Instance.DeselectAnchor();
            }

            // destroy anchor
            Destroy(hit.transform.parent.gameObject);
            break;
        }
    }

    public static AnchorController GetAnchor(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.01f);

        foreach (Collider2D hit in hits)
        {
            if (hit.transform.parent.CompareTag("Anchor")) return hit.gameObject.GetComponent<AnchorController>();
        }

        return null;
    }

    public static AnchorController GetAnchor(float mx, float my) => GetAnchor(new(mx, my));
}