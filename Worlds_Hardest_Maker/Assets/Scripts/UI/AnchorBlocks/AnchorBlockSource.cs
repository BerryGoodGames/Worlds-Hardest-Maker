using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockSource : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject anchorBlockPrefab;
    [SerializeField] private bool active = true;

    public void CreateNew()
    {
        Vector2 position = transform.position;

        GameObject anchorBlock = Instantiate(anchorBlockPrefab,
            position,
            Quaternion.identity,
            ReferenceManager.Instance.AnchorBlockChainContainer);

        // activate restriction
        UIRestrictInRectTransform restrict = anchorBlock.GetComponent<UIRestrictInRectTransform>();
        restrict.RectTransform = ReferenceManager.Instance.AnchorBlockChainContainer;

        // rebuild
        AnchorBlockRebuilder rebuilder = anchorBlock.GetComponent<AnchorBlockRebuilder>();
        rebuilder.RebuildLayout();

        // transition to dragging
        anchorBlock.GetComponent<AnchorBlockDragDrop>().BeginDrag();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (active && eventData.button == PointerEventData.InputButton.Left) CreateNew();
    }
}