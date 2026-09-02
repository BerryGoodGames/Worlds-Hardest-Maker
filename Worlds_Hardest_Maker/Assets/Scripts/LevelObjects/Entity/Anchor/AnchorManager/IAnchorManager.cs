// TODO: god interface, split
public interface IAnchorManager
{
    public AnchorController SelectedAnchor { get; }
    
    public void Remove(AnchorController anchor);
    public void UpdateBlockListInSelectedAnchor();
    public void CheckStartRotatingWarnings();
    public void CheckStackOverflowWarnings();
    public void UpdateSelectedAnchorLines();
    public void DeselectAnchor();
}