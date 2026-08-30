using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace WorldsHardestMaker.Selection
{
    [Serializable]
    public class SelectionPreviewService : IDisposable
    {    
        [SerializeField] [InitializationField] [MustBeAssigned] private FillPreviewCoordinator fillPreviewPrefab;
        [SerializeField] [InitializationField] [MustBeAssigned] private MouseOverUIRect fillOptionMouseOver;
        [SerializeField] [InitializationField] [MustBeAssigned] private Transform container;
    
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
            if (!LevelSessionEditManager.Instance.CurrentEditMode.ShowFillPreview) return;

            EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
            IOutlineConnectivityProvider batchProvider = BuildBatchProvider(area, currentEditMode);

            foreach (Vector2 pos in area.Positions)
            {
                FillPreviewCoordinator fillPreview = Object.Instantiate(
                    fillPreviewPrefab, pos, Quaternion.identity,
                    container
                );

                diContainer.InjectGameObject(fillPreview.gameObject);

                fillPreview.UpdateSprite();
                fillPreview.UpdateRotation();
                fillPreview.SetOutlineBatchProvider(batchProvider);
                fillPreview.UpdateOutline();
            }
        }
        
        private static IOutlineConnectivityProvider BuildBatchProvider(SelectionArea area, EditMode editMode)
        {
            if (editMode is not FieldMode fieldMode) return NullOutlineConnectivityProvider.Instance;

            Dictionary<Vector2Int, string> positionToTag = new();
            foreach (Vector2 pos in area.Positions) positionToTag[Vector2Int.RoundToInt(pos)] = fieldMode.Tag;

            // Fill previews are static and already in world/matrix space, so no moving origin is needed.
            return new BatchOutlineConnectivityProvider(positionToTag);
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
            // Activate first: instantiating children under an inactive parent defers their
            // Awake/OnEnable/Start, which previously left PreviewOutlineComponent's line
            // container uninitialized when Spawn() tried to use it immediately.
            container.gameObject.SetActive(true);

            if (container.childCount == 0) Spawn(selectionAreaProvider.GetArea());
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
}