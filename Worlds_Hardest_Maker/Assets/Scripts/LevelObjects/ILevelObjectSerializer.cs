using System.Collections.Generic;

public interface ILevelObjectSerializer
{
    public IEnumerable<Data> Serialize();
}