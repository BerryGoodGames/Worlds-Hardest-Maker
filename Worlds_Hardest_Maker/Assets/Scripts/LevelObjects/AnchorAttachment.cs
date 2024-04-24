using System;
using MyBox;
using UnityEngine;

public class AnchorAttachment : MonoBehaviour
{
    public const int INTERNAL_LAYER_OFFSET = 3000;

    [ReadOnly] public AnchorController Anchor;
    [ReadOnly] public LevelObjectController Controller;
    [ReadOnly] public AnchorAttachable AnchorAttachable;
    [ReadOnly] public bool HasAnimator;
    [SerializeField] [ReadOnly] private Animator animator;
    [Space] [ReadOnly] public int SortingLayerID;
    [ReadOnly] public int OrderInLayer;
    [ReadOnly] public float Opacity;

    public void MergeToLayer()
    {
        AnchorAttachable.MainSprite.sortingLayerName = LayerManager.Instance.SortingLayers.AnchorAbove;
        if (AnchorAttachable.HasOutline)
            AnchorAttachable.OutlineComp.LineRenderers.ForEach(
                line =>
                {
                    if (line != null) line.sortingLayerName = LayerManager.Instance.SortingLayers.AnchorAbove;
                }
            );
    }

    public void ResetLayer()
    {
        AnchorAttachable.MainSprite.sortingLayerName = LayerManager.Instance.SortingLayers.AnchorBelow;
        if (AnchorAttachable.HasOutline)
            AnchorAttachable.OutlineComp.LineRenderers.ForEach(
                line =>
                {
                    if (line != null) line.sortingLayerName = LayerManager.Instance.SortingLayers.AnchorBelow;
                }
            );
    }

    private void Start()
    {
        Anchor.Attachments.Add(this);

        if (!TryGetComponent(out AnchorAttachable))
        {
            Debug.LogError("Object with anchor attachment is not anchor attachable");
            return;
        }

        Controller = GetComponent<LevelObjectController>();
        if (Controller == null)
        {
            Controller = GetComponentInChildren<LevelObjectController>();
        }
        
        if (Controller == null) Debug.LogWarning("Could not assign level object controller because none was found");

        HasAnimator = TryGetComponent(out animator);
        
        SortingLayerID = AnchorAttachable.MainSprite.sortingLayerID;
        OrderInLayer = AnchorAttachable.MainSprite.sortingOrder;
        Opacity = AnchorAttachable.MainSprite.color.a;

        int sortingOrder = Array.IndexOf(LayerManager.Instance.AllSortingLayerIDs, SortingLayerID) * INTERNAL_LAYER_OFFSET
                           + Math.Min(OrderInLayer, INTERNAL_LAYER_OFFSET - 1);

        string sortingLayerName = LayerManager.Instance.SortingLayers.AnchorAbove;

        AnchorAttachable.MainSprite.sortingOrder = sortingOrder;
        AnchorAttachable.MainSprite.sortingLayerName = sortingLayerName;
        if (AnchorAttachable.HasOutline)
        {
            UpdateOutlineLayers();

            AnchorAttachable.OutlineComp.OnUpdateOutline += UpdateOutlineLayers;
        }

        return;

        void UpdateOutlineLayers() =>
            AnchorAttachable.OutlineComp.LineRenderers.ForEach(
                line =>
                {
                    line.sortingOrder = sortingOrder;
                    line.sortingLayerName = AnchorAttachManager.Instance.InAttachMode
                        ? LayerManager.Instance.SortingLayers.AnchorAbove
                        : LayerManager.Instance.SortingLayers.AnchorBelow;
                }
            );
    }

    private void OnDestroy()
    {
        Anchor.Attachments.Remove(this);
    }
}