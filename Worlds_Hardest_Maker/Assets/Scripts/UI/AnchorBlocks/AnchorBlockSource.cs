using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockSource : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject anchorBlockPrefab;
    [SerializeField] private bool active = true;

    private Vector3 anchorBlockSize;

    private void Start()
    {
        anchorBlockSize = GetComponent<RectTransform>().sizeDelta;
    }

    public void CreateNew()
    {
        Vector2 position = transform.position + anchorBlockSize / 3;
        
        GameObject anchorBlock = Instantiate(anchorBlockPrefab,
            position,
            Quaternion.identity,
            ReferenceManager.Instance.AnchorBlockStringContainer);
        anchorBlock.GetComponent<BlockDragDrop>().BeginDrag();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (active && eventData.button == PointerEventData.InputButton.Left) CreateNew();
    }
}