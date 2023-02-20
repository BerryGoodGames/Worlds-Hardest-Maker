using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockSource : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject anchorBlockPrefab;
    [SerializeField] private Vector2 offset = new(-10, 10);

    public void CreateNew()
    {
        Instantiate(anchorBlockPrefab, transform.position + (Vector3)offset, Quaternion.identity, container);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        CreateNew();
    }
}
