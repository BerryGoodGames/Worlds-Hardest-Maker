using Photon.Pun;
using UnityEngine;

public class AnchorManager : MonoBehaviour
{
    public static AnchorManager Instance { get; private set; }

    private static readonly int selected = Animator.StringToHash("Selected");

    public AnchorController selectedAnchor;

    #region get, set, remove
    /// <summary>
    ///     places anchor at position
    /// </summary>
    /// <param name="mx">x position of anchor</param>
    /// <param name="my">y position of anchor</param>
    public GameObject SetAnchor(float mx, float my)
    {
        return SetAnchor(new(mx, my));
    }

    /// <summary>
    ///     places anchor at position
    /// </summary>
    /// <param name="pos">position of anchor</param>
    public GameObject SetAnchor(Vector2 pos)
    {
        if (GetAnchor(pos) != null) return null;

        GameObject anchor = MultiplayerManager.Instance.Multiplayer
            ? PhotonNetwork.Instantiate("Anchor", pos, Quaternion.identity)
            : Instantiate(PrefabManager.Instance.anchor, pos, Quaternion.identity,
                ReferenceManager.Instance.anchorContainer);

        SelectAnchor(anchor.GetComponent<AnchorControllerParent>().child);

        return anchor;
    }

    /// <summary>
    ///     removes anchor at position
    /// </summary>
    /// <param name="mx">x position of anchor</param>
    /// <param name="my">y position of anchor</param>
    [PunRPC]
    public void RemoveAnchor(float mx, float my)
    {
        RemoveAnchor(new(mx, my));
    }

    /// <summary>
    ///     removes anchor at position
    /// </summary>
    /// <param name="pos">position of anchor</param>
    public static void RemoveAnchor(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.5f, 128);

        foreach (Collider2D hit in hits)
        {
            if (!hit.transform.parent.CompareTag("Anchor")) continue;

            Destroy(hit.transform.parent.gameObject);
            break;
        }
    }

    /// <param name="pos">position of anchor</param>
    /// <returns>anchor at position</returns>
    public static AnchorController GetAnchor(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.5f);

        foreach (Collider2D hit in hits)
        {
            if (hit.transform.parent.CompareTag("Anchor"))
            {
                return hit.gameObject.GetComponent<AnchorController>();
            }
        }

        return null;
    }

    public static AnchorController GetAnchor(float mx, float my)
    {
        return GetAnchor(new(mx, my));
    }
    #endregion

    public void SelectAnchor(Vector2 pos)
    {
        AnchorController anchor = GetAnchor(pos);
        if (anchor == null) return;

        Instance.SelectAnchor(anchor);
    }

    public void SelectAnchor(AnchorController anchor)
    {
        if (selectedAnchor != null)
        {
            selectedAnchor.animator.SetBool(selected, false);
        }

        if (selectedAnchor == anchor)
        {
            DeselectAnchor();
            return;
        }

        selectedAnchor = anchor;
        selectedAnchor.animator.SetBool(selected, true);
    }

    public void DeselectAnchor()
    {
        if (selectedAnchor == null) return;

        selectedAnchor.animator.SetBool(selected, false);
        selectedAnchor = null;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}