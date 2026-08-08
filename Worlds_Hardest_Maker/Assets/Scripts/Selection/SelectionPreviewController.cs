using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

public class SelectionPreviewController : IDisposable
{
    private readonly EventBus eventBus;
    private readonly IObjectResolver diContainer;
    private readonly IFillRangeProvider fillRangeProvider;

    public SelectionPreviewController(EventBus eventBus, IObjectResolver diContainer, IFillRangeProvider fillRangeProvider)
    {
        this.eventBus = eventBus;
        this.diContainer = diContainer;
        this.fillRangeProvider = fillRangeProvider;

        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Subscribe<SelectionClearedEvent>(OnSelectionCleared);
    }
    
    private void OnEditModeChange(EditModeChangeEvent evt) => UpdateFillPreviews();
    private void OnSelectionCleared(SelectionClearedEvent evt) => Clear();
    
    public void UpdateFillPreviews()
    {
        if (ReferenceManager.Instance.FillPreviewContainer.childCount == 0) return;

        Clear();
        InstantiatePreview(fillRangeProvider.GetFillRange());
    }

    private void InstantiatePreview(List<Vector2> range)
    {
        // set new previews, only if edit mode not in NoFillPreviewModes
        if (!LevelSessionEditManager.Instance.CurrentEditMode.ShowFillPreview) return;

        foreach (Vector2 pos in range)
        {
            FillPreviewCoordinator fillPreview = Object.Instantiate(
                PrefabManager.Instance.FillPreview, pos, Quaternion.identity,
                ReferenceManager.Instance.FillPreviewContainer
            );

            diContainer.InjectGameObject(fillPreview.gameObject);

            fillPreview.UpdateSprite();
            fillPreview.UpdateRotation();
        }
    }

    public void Clear()
    {
        if (!LevelSessionManager.Instance.IsEdit) return;

        // destroy selection previews
        foreach (Transform preview in ReferenceManager.Instance.FillPreviewContainer)
        {
            Object.Destroy(preview.gameObject);
        }
    }

    public void SetPreviewVisible()
    {
        if (ReferenceManager.Instance.FillPreviewContainer.childCount == 0) InstantiatePreview(fillRangeProvider.GetFillRange());

        ReferenceManager.Instance.FillPreviewContainer.gameObject.SetActive(true);
    }

    public void SetPreviewInvisible()
    {
        ReferenceManager.Instance.FillPreviewContainer.gameObject.SetActive(false);
    }

    public void Dispose()
    {
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Unsubscribe<SelectionClearedEvent>(OnSelectionCleared);
    }
}