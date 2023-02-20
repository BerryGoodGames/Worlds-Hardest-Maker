using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockSource : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject anchorBlockPrefab;
    [SerializeField] private Vector2 offset = new(-10, 10);
    [SerializeField] private bool active;

    public void CreateNew()
    {
        Instantiate(anchorBlockPrefab, transform.position + (Vector3)offset, Quaternion.identity, ReferenceManager.Instance.AnchorBlockContainer);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if(active) CreateNew();
    }
}
