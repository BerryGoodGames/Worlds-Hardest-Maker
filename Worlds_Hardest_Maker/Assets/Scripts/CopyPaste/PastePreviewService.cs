using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

[Serializable]
public class PastePreviewService
{
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform container;
    [SerializeField] [InitializationField] [MustBeAssigned] private PastePreviewCoordinator pastePreviewPrefab;

    private IObjectResolver diContainer;
    
    public void Initialize(IObjectResolver diContainer)
    {
        this.diContainer = diContainer;
    }
    
    public void CreatePreview(List<CopyData> clipBoard)
    {
        ClearPreview();
        
        foreach (CopyData copyData in clipBoard)
        {
            Quaternion rotation = copyData.Data.GetType() == typeof(FieldData)
                ? Quaternion.Euler(0, 0, ((FieldData)copyData.Data).Rotation)
                : Quaternion.identity;
            
            PastePreviewCoordinator preview = Object.Instantiate(
                pastePreviewPrefab, Vector2.zero, rotation,
                container
            );
            
            diContainer.InjectGameObject(preview.gameObject);
            
            preview.transform.localPosition = copyData.RelativePos;
            
            preview.ApplyCopyData(copyData);
        }
    }
    
    public void ClearPreview()
    {
        foreach (Transform child in container) Object.Destroy(child.gameObject);
        container.position = Vector3.zero;
    }
}