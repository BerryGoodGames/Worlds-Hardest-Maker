public interface IPreviewDataReceiver<in T> where T : IPreviewData
{
    void ApplyPreviewData(T previewData);
}