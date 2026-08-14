using System;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SelectionOutlineController : IDisposable
{
    [SerializeField] [PositiveValueOnly] private float weight = 0.1f;
    [SerializeField] [PositiveValueOnly] private float animationDuration = 0.1f;
    [SerializeField] private Color color = Color.black;
    
    private EventBus eventBus;
    private IDrawService drawService;
    
    private GameObject outline;
    private LineAnimator outlineAnimator;

    private void OnSelectionStarted(SelectionStartedEvent evt) => InitializeOutline(evt.Start);
    private void OnSelectionUpdated(SelectionUpdatedEvent evt) => AnimateOutline(evt.Start, evt.End);
    private void OnSelectionCleared(SelectionClearedEvent evt) => Clear();

    public void SetEventBus(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<SelectionStartedEvent>(OnSelectionStarted);
        eventBus.Subscribe<SelectionUpdatedEvent>(OnSelectionUpdated);
        eventBus.Subscribe<SelectionClearedEvent>(OnSelectionCleared);
    }

    private void InitializeOutline(Vector2 start)
    {
        Clear();
        
        drawService.SetWeight(weight);
        drawService.SetFill(color);
        
        drawService.SetLayerName(LayerManager.Instance.SortingLayers.Line);
        drawService.SetOrderInLayer(0);
        outline = drawService.DrawRect(
            start.x + 0.5f,
            start.y + 0.5f,
            -1,
            -1,
            false, ReferenceManager.Instance.SelectionOutlineContainer
        ).gameObject;
        
        outlineAnimator = outline.AddComponent<LineAnimator>();
    }
    
    private void AnimateOutline(Vector2 start, Vector2 end)
    {
        if (outlineAnimator == null) return;
        
        // get position and stuff
        float width = end.x - start.x;
        float height = end.y - start.y;
        
        float x = width > 0 ? start.x - 0.5f : start.x + 0.5f;
        float y = height > 0 ? start.y - 0.5f : start.y + 0.5f;
        float w = width > 0 ? width + 1 : width - 1;
        float h = height > 0 ? height + 1 : height - 1;
        
        List<Vector2> lineVertices = new(
            new Vector2[]
            {
                new(x, y),
                new(x + w, y),
                new(x + w, y + h),
                new(x, y + h),
                new(x, y),
            }
        );
        
        outlineAnimator.AnimateAllPoints(lineVertices, animationDuration, Ease.OutSine);
    }

    private void Clear()
    {
        if (outline != null) Object.Destroy(outline);

        outline = null;
        outlineAnimator = null;
    }

    public void Dispose()
    {
        eventBus.Unsubscribe<SelectionStartedEvent>(OnSelectionStarted);
        eventBus.Unsubscribe<SelectionUpdatedEvent>(OnSelectionUpdated);
        eventBus.Unsubscribe<SelectionClearedEvent>(OnSelectionCleared);
    }
}