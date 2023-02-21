using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockSource : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject anchorBlockPrefab;
    [SerializeField] private bool active;

    public void CreateNew()
    {
        GameObject anchorBlock = Instantiate(anchorBlockPrefab, transform.position, Quaternion.identity, ReferenceManager.Instance.AnchorBlockStringContainer);
        anchorBlock.GetComponent<BlockDragDrop>().BeginDrag();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if(active) CreateNew();
    }
}
