using MyBox;
using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public abstract partial class AnchorBlockController : MonoBehaviour
{
    private readonly Vector2 duplicateOffset = new(-10, 10);

    [Separator("General")] public bool IsLocked;
    public bool IsSource;

    [Space] [SerializeField] private GameObject lockIconContainer;

    [HideInInspector] public AnchorBlockDragDrop AnchorBlockDragDropComp;
    private UIRestrictInRectTransform restrict;

    public abstract AnchorBlock GetAnchorBlock(AnchorController anchorController);

    private void MainStart()
    {
        AnchorBlockDragDropComp = GetComponent<AnchorBlockDragDrop>();
        restrict = GetComponent<UIRestrictInRectTransform>();

        if (IsLocked)
        {
            if(TryGetComponent(out AnchorBlockQuickMenu quickMenu)) quickMenu.Active = false;

            if (TryGetComponent(out AnchorBlockDragDrop dragDrop)) dragDrop.IsLocked = true;
        }
        
        IsSource = TryGetComponent(out AnchorBlockSource _);

        // enable lock icon if unmovable and not a source
        lockIconContainer.SetActive(IsLocked && !IsSource);
    }

    public void Delete()
    {
        bool wasInChain = IsInChain();

        // update scrollbar of anchor block container
        transform.SetParent(null);

        ReferenceManager.Instance.AnchorBlockFitter.CheckForChanges();

        if (wasInChain)
        {
            ReferenceManager.Instance.MainChainController.UpdateChildrenArray();

            ReferenceManager.Instance.AnchorBlockConnectorController.UpdateY();

            AnchorManager.Instance.UpdateSelectedAnchorLines();
        }

        Destroy(gameObject);
    }

    public void Duplicate() =>
        Instantiate(gameObject, transform.position + (Vector3)duplicateOffset, Quaternion.identity,
            ReferenceManager.Instance.AnchorBlockChainContainer);

    public void TrimFromCurrentChain()
    {
        if (!IsInChain(out ChainController _)) return;

        // push anchor block to container and remove from string
        transform.SetParent(ReferenceManager.Instance.AnchorBlockChainContainer);
        restrict.enabled = true;

        // AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

        ReferenceManager.Instance.AnchorBlockConnectorController.UpdateY();
    }

    /// <summary>
    /// Gets sibling index inside of string while ignoring preview object
    /// </summary>
    public int GetChainIndex()
    {
        int res = 0;
        foreach (Transform t in transform.parent)
        {
            if (t.CompareTag("StartBlock")) continue;
            if (t.CompareTag("AnchorBlockPreview")) continue;
            if (t == transform) break;

            res++;
        }

        return res;
    }

    public bool IsInChain(out ChainController chainController) =>
        transform.parent.TryGetComponent(out chainController);

    public bool IsInChain()
    {
        Transform parent = transform.parent;
        return parent != null && parent.TryGetComponent(out ChainController _);
    }


    private void Start()
    {
        MainStart();
        HoveringStart();
    }
}