using Photon.Pun;
using UnityEngine;

/// <summary>
///     Manages the anchors (duh)
/// </summary>
public class AnchorManagerOld : MonoBehaviour
{
    public static AnchorManagerOld Instance { get; private set; }

    private GameObject selectedAnchor;

    public PathEditorControllerOld pathEditorControllerOld;

    public GameObject SelectedAnchor
    {
        get => selectedAnchor;
        set => SelectAnchor(value);
    }

    [HideInInspector] public PathControllerOld selectedPathControllerOld;

    private static readonly int selected = Animator.StringToHash("Selected");


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
    public static GameObject GetAnchor(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.5f);

        foreach (Collider2D hit in hits)
        {
            if (hit.transform.parent.CompareTag("Anchor"))
            {
                return hit.gameObject;
            }
        }

        return null;
    }

    public static GameObject GetAnchor(float mx, float my)
    {
        return GetAnchor(new(mx, my));
    }

    public static void SelectAnchor(Vector2 pos)
    {
        GameObject anchor = GetAnchor(pos);
        if (anchor == null) return;

        Instance.SelectedAnchor = anchor;
        if (EditModeManager.Instance.CurrentEditMode != EditMode.BALL)
            EditModeManager.Instance.CurrentEditMode = EditMode.ANCHOR;

        Animator anim = anchor.GetComponent<Animator>();
        anim.SetBool(selected, true);
    }

    public void SelectAnchor(GameObject anchor)
    {
        if (anchor.TryGetComponent(out AnchorControllerOld _))
        {
            if (selectedAnchor != null)
            {
                selectedAnchor.GetComponent<Animator>().SetBool(selected, false);

                selectedPathControllerOld.ClearLines();
                selectedPathControllerOld.drawLines = false;
            }

            selectedAnchor = anchor;
            selectedAnchor.GetComponent<Animator>().SetBool(selected, true);
            selectedPathControllerOld = selectedAnchor.GetComponent<PathControllerOld>();
            selectedPathControllerOld.drawLines = true;
        }

        pathEditorControllerOld.UpdateUI();
    }

    public static void ResetPathEditorPosition()
    {
        Instance.pathEditorControllerOld.ResetPosition();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}