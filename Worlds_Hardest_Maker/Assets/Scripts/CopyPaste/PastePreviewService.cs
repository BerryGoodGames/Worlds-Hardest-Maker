using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace WorldsHardestMaker.CopyPaste
{
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

            IOutlineConnectivityProvider batchProvider = BuildBatchProvider(clipBoard);

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
                preview.SetOutlineBatchProvider(batchProvider);
                preview.UpdateOutline(copyData.Data.GetEditMode());
            }
        }

        private IOutlineConnectivityProvider BuildBatchProvider(List<CopyData> clipboard)
        {
            Dictionary<Vector2Int, string> relativePositionToTag = new();

            foreach (CopyData copyData in clipboard)
            {
                if (copyData.Data.GetEditMode() is not FieldMode fieldMode) continue;

                relativePositionToTag[Vector2Int.RoundToInt(copyData.RelativePos)] = fieldMode.Tag;
            }

            // The paste container moves with the mouse every frame, so positions are relative to it.
            return new BatchOutlineConnectivityProvider(relativePositionToTag, () => container.position);
        }
    
        public void ClearPreview()
        {
            foreach (Transform child in container) Object.Destroy(child.gameObject);
            container.position = Vector3.zero;
        }
    }
}