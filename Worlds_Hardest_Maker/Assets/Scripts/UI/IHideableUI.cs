public interface IHideableUI
{
    public bool IsHidden { get; }

    public void SetHidden(bool hidden);
}