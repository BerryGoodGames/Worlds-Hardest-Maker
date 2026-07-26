using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public partial class SelectionManager
{
    private void UpdateFillPreviews()
    {
        if (ReferenceManager.Instance.FillPreviewContainer.childCount == 0) return;
        
        DestroyPreview();
        InitSelectedPreview();
    }
    
    private void InitPreview(List<Vector2> range)
    {
        // set new previews, only if edit mode not in NoFillPreviewModes
        if (!LevelSessionEditManager.Instance.CurrentEditMode.ShowFillPreview) return;
        
        foreach (Vector2 pos in range)
        {
            FillPreviewCoordinator fillPreview = Instantiate(
                PrefabManager.Instance.FillPreview, pos, Quaternion.identity,
                ReferenceManager.Instance.FillPreviewContainer
            );
            
            diContainer.InjectGameObject(fillPreview.gameObject);
            
            fillPreview.UpdateSprite();
            fillPreview.UpdateRotation();
        }
    }
    
    private void DestroyPreview()
    {
        if (!LevelSessionManager.Instance.IsEdit) return;
        
        // destroy selection previews
        foreach (Transform preview in ReferenceManager.Instance.FillPreviewContainer)
        {
            Destroy(preview.gameObject);
        }
    }
    
    private void InitSelectedPreview() => InitPreview(GetCurrentFillRange());
    
    private void SetPreviewVisible()
    {
        if (ReferenceManager.Instance.FillPreviewContainer.childCount == 0) InitSelectedPreview();
        
        ReferenceManager.Instance.FillPreviewContainer.gameObject.SetActive(true);
    }
    
    private static void SetPreviewInvisible() => ReferenceManager.Instance.FillPreviewContainer.gameObject.SetActive(false);
    
    public void ResetPreview()
    {
        // reset preview
        DestroyPreview();
        
        // enable placement preview
        if (!LevelSessionEditManager.Instance.Playing) placementPreview.Show();
        
        // reset selection marking
        if (selectionOutline != null) Destroy(selectionOutline);
    }
}