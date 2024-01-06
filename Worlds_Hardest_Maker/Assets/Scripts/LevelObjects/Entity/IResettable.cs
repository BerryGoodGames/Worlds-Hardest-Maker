public interface IResettable
{
    void ResetState();

    public sealed void Subscribe()
    {
        PlayManager.Instance.OnLevelReset += ResetState;
    }
}
