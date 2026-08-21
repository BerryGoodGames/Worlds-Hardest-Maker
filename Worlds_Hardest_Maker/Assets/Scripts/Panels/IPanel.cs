public interface IPanel : IHideableUI
{
    public bool IsOpen { get; }
    public bool CloseOnEscape { get; }
    public PanelExclusionGroup ExclusionGroup { get; }

    public void SetOpen(bool open);
}