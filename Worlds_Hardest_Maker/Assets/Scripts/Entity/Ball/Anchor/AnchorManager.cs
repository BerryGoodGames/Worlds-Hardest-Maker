using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// Manages the anchors (duh)
/// </summary>
public class AnchorManager : MonoBehaviour
{
    public static AnchorManager Instance { get; private set; }

    private GameObject selectedAnchor;

    public PathEditorController pathEditorController;

    public GameObject SelectedAnchor
    {
        get => selectedAnchor;
        set => SelectAnchor(value);
    }

    [HideInInspector] public PathController selectedPathController;

    private static readonly int Selected = Animator.StringToHash("Selected");


    /// <summary>
    /// places anchor at position
    /// </summary>
    /// <param name="mx">x position of anchor</param>
    /// <param name="my">y position of anchor</param>
    public GameObject SetAnchor(float mx, float my)
    {
        return SetAnchor(new(mx, my));
    }
    /// <summary>
    /// places anchor at position
    /// </summary>
    /// <param name="pos">position of anchor</param>
    public GameObject SetAnchor(Vector2 pos)
    {
        if (GetAnchor(pos) != null) return null;

        GameObject anchor = GameManager.Instance.Multiplayer
            ? PhotonNetwork.Instantiate("Anchor", pos, Quaternion.identity)
            : Instantiate(PrefabManager.Instance.Anchor, pos, Quaternion.identity,
                ReferenceManager.Instance.AnchorContainer);

        return anchor;
    }

    /// <summary>
    /// removes anchor at position
    /// </summary>
    /// <param name="mx">x position of anchor</param>
    /// <param name="my">y position of anchor</param>
    [PunRPC]
    public void RemoveAnchor(float mx, float my)
    {
        RemoveAnchor(new(mx, my));
    }

    /// <summary>
    /// removes anchor at position
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
        if(GameManager.Instance.CurrentEditMode != EditMode.BALL)
            GameManager.Instance.CurrentEditMode = EditMode.ANCHOR;

        Animator anim = anchor.GetComponent<Animator>();
        anim.SetBool(Selected, true);
    }

    public void SelectAnchor(GameObject anchor)
    {
        if (anchor.TryGetComponent(out AnchorController _))
        {
            if (selectedAnchor != null)
            {
                selectedAnchor.GetComponent<Animator>().SetBool(Selected, false);

                selectedPathController.ClearLines();
                selectedPathController.drawLines = false;
            }

            selectedAnchor = anchor;
            selectedAnchor.GetComponent<Animator>().SetBool(Selected, true);
            selectedPathController = selectedAnchor.GetComponent<PathController>();
            selectedPathController.drawLines = true;
        }

        pathEditorController.UpdateUI();
    }

    public static void ResetPathEditorPosition()
    {
        Instance.pathEditorController.ResetPosition();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
