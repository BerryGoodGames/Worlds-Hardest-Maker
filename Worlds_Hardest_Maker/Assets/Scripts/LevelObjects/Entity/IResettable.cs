public interface IResettable
{
    void ResetState();

    public sealed void Subscribe()
    {
        PlayManager.Instance.OnLevelReset += ResetState;
    }

    public sealed void Unsubscribe()
    {
        PlayManager.Instance.OnLevelReset -= ResetState;
    }
}
