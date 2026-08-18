public interface IPanel
{
    public bool Open { get; }
    public bool Hidden { get; }
    public bool CloseOnEscape { get; }

    public void SetOpen(bool open, bool noAnimation = false);

    public void SetHidden(bool hidden, bool noAnimation = false);
}