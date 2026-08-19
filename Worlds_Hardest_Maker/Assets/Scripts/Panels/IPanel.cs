public interface IPanel
{
    public bool Open { get; }
    public bool Hidden { get; }
    public bool CloseOnEscape { get; }
    public PanelExclusionGroup ExclusionGroup { get; }

    public void SetOpen(bool open);

    public void SetHidden(bool hidden);
}