using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitDropdownMap<TEnum> : IEnumerable<KeyValuePair<string, TEnum>>
{
    private Dictionary<string, TEnum> options;
    
    public TEnum this[string name] => options[name];
    
    public void Add(string label, TEnum option)
    {
        options.Add(label, option);
    }

    public string GetLabel(TEnum option)
    {
        return options.FirstOrDefault(x => EqualityComparer<TEnum>.Default.Equals(x.Value, option)).Key;
    }

    public IEnumerator<KeyValuePair<string, TEnum>> GetEnumerator()
    {
        return options.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}