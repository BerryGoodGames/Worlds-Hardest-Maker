using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockSource : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject anchorBlockPrefab;
    [SerializeField] private readonly bool active = true;

    public void CreateNew()
    {
        GameObject anchorBlock = Instantiate(anchorBlockPrefab,
            transform.position + new Vector3(0, anchorBlockPrefab.GetComponent<RectTransform>().sizeDelta.y / 2),
            Quaternion.identity,
            ReferenceManager.Instance.AnchorBlockStringContainer);
        anchorBlock.GetComponent<BlockDragDrop>().BeginDrag();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (active && eventData.button == PointerEventData.InputButton.Left) CreateNew();
    }
}