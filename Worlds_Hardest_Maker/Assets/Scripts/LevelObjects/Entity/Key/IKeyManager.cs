using System.Collections.Generic;

public interface IKeyManager
{
    public IReadOnlyList<KeyController> CollectedKeys { get; }
    
    public void CollectKey(KeyController key);
    public void UncollectKey(KeyController key);
    public bool AllKeysCollected(KeyColor color);
    public void RemoveCollectedKeyNulls();
    public void ClearCollectedKeys();
}