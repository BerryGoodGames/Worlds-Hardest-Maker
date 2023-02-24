using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class AnchorManager : MonoBehaviour
{
    public static AnchorManager Instance { get; private set; }

    private static readonly int selected = Animator.StringToHash("Selected");
    private static readonly int playing = Animator.StringToHash("Playing");

    [SerializeField]
    private AnchorController selectedAnchor;

    public AnchorController SelectedAnchor
    {
        get => selectedAnchor;
        set
        {
            UpdateSelectedAnchor();
            selectedAnchor = value;
        }
    }

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
            : Instantiate(PrefabManager.Instance.Anchor, pos, Quaternion.identity,
                ReferenceManager.Instance.AnchorContainer);

        SelectAnchor(anchor.GetComponent<AnchorControllerParent>().Child);

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

    #region select, deselect

    public void SelectAnchor(Vector2 pos)
    {
        AnchorController anchor = GetAnchor(pos);
        if (anchor == null) return;

        Instance.SelectAnchor(anchor);
    }

    public void SelectAnchor(AnchorController anchor)
    {
        if (SelectedAnchor != null)
        {
            SelectedAnchor.Animator.SetBool(selected, false);
        }

        if (SelectedAnchor == anchor)
        {
            DeselectAnchor();
            return;
        }

        SelectedAnchor = anchor;
        SelectedAnchor.Animator.SetBool(selected, true);

        ReferenceManager.Instance.AnchorEditorButtonPanelTween.Set(true);

        ReferenceManager.Instance.LevelSettingsButtonPanelTween.Set(false);
        ReferenceManager.Instance.LevelSettingsPanelTween.Set(false);
    }

    public void DeselectAnchor()
    {
        if (SelectedAnchor == null) return;

        SelectedAnchor.Animator.SetBool(selected, false);
        SelectedAnchor.Animator.SetBool(playing, EditModeManager.Instance.Playing);
        SelectedAnchor = null;

        ReferenceManager.Instance.AnchorEditorButtonPanelTween.Set(false);
        ReferenceManager.Instance.AnchorEditorPanelTween.Set(false);

        ReferenceManager.Instance.LevelSettingsButtonPanelTween.Set(true);
    }

    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        EditModeManager.Instance.OnPlay += UpdateSelectedAnchor;
    }

    private void UpdateSelectedAnchor()
    {
        if (selectedAnchor == null) return;

        print("update");
        selectedAnchor.Blocks = new(ReferenceManager.Instance.MainStringController.GetAnchorBlocks(selectedAnchor));
    }
}