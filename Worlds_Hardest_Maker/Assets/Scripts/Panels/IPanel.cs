public interface IPanel
{
    public bool IsOpen { get; }
    public bool IsHidden { get; }
    public bool CloseOnEscape { get; }
    public PanelExclusionGroup ExclusionGroup { get; }

    public void SetOpen(bool open);

    public void SetHidden(bool hidden);
}