using System;
using System.Collections.Generic;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

[Serializable]
public class SelectionPreviewController : IDisposable
{    
    [SerializeField] [InitializationField] [Required] private MouseOverUIRect fillOptionMouseOver;
    [SerializeField] [InitializationField] [Required] private Transform container;
    
    private EventBus eventBus;
    private IObjectResolver diContainer;
    private ISelectionAreaProvider selectionAreaProvider;

    public void Initialize(EventBus eventBus, IObjectResolver diContainer, ISelectionAreaProvider selectionAreaProvider)
    {
        this.eventBus = eventBus;
        this.diContainer = diContainer;
        this.selectionAreaProvider = selectionAreaProvider;

        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Subscribe<SelectionEndedEvent>(OnSelectionEnded);
        eventBus.Subscribe<SelectionClearedEvent>(OnSelectionCleared);
        
        fillOptionMouseOver.OnHovered += SetVisible;
        fillOptionMouseOver.OnUnhovered += SetInvisible;
    }
    
    private void OnEditModeChange(EditModeChangeEvent evt) => Refresh();
    private void OnSelectionEnded(SelectionEndedEvent evt) => Refresh();
    private void OnSelectionCleared(SelectionClearedEvent evt) => Clear();
    
    public void Refresh()
    {
        if (container.childCount == 0) return;

        Clear();
        Spawn(selectionAreaProvider.GetArea());
    }

    private void Spawn(SelectionArea area)
    {
        // set new previews, only if edit mode not in NoFillPreviewModes
        if (!LevelSessionEditManager.Instance.CurrentEditMode.ShowFillPreview) return;

        foreach (Vector2 pos in area.Positions)
        {
            FillPreviewCoordinator fillPreview = Object.Instantiate(
                PrefabManager.Instance.FillPreview, pos, Quaternion.identity,
                container
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
        foreach (Transform preview in container)
        {
            Object.Destroy(preview.gameObject);
        }
    }

    public void SetVisible()
    {
        if (container.childCount == 0) Spawn(selectionAreaProvider.GetArea());

        container.gameObject.SetActive(true);
    }

    public void SetInvisible()
    {
        container.gameObject.SetActive(false);
    }

    public void Dispose()
    {
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Unsubscribe<SelectionEndedEvent>(OnSelectionEnded);
        eventBus.Unsubscribe<SelectionClearedEvent>(OnSelectionCleared);
        
        fillOptionMouseOver.OnHovered -= SetVisible;
        fillOptionMouseOver.OnUnhovered -= SetInvisible;
    }
}