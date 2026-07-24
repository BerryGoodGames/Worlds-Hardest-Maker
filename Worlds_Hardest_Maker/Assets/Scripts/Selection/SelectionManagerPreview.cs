using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public partial class SelectionManager
{
    private void RemakePreview()
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
            GameObject preview = Instantiate(
                PrefabManager.Instance.FillPreview, pos, Quaternion.identity,
                ReferenceManager.Instance.FillPreviewContainer
            );
            
            IObjectResolver.InjectGameObject(preview);
            
            PreviewController c = preview.GetComponent<PreviewController>();
            c.Awake_();
            c.UpdateSprite();
            c.UpdateRotation(smooth: false);
        }
    }
    
    private static void DestroyPreview()
    {
        if (!LevelSessionManager.Instance.IsEdit) return;
        
        // destroy selection previews
        foreach (Transform preview in ReferenceManager.Instance.FillPreviewContainer) Destroy(preview.gameObject);
    }
    
    private void InitSelectedPreview() => InitPreview(GetCurrentFillRange());
    
    public static void UpdatePreviewRotation()
    {
        foreach (Transform preview in ReferenceManager.Instance.FillPreviewContainer) preview.GetComponent<PreviewController>().UpdateRotation();
    }
    
    public static void UpdatePreviewSprite()
    {
        foreach (Transform preview in ReferenceManager.Instance.FillPreviewContainer) preview.GetComponent<PreviewController>().UpdateSprite();
    }
    
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
        if (!LevelSessionEditManager.Instance.Playing) ReferenceManager.Instance.PlacementPreview.Activate();
        
        // reset selection marking
        if (selectionOutline != null) Destroy(selectionOutline);
    }
}