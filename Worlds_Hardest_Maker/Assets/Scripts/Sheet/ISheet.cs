using System;
using UnityEngine;

public interface ISheet : IEquatable<ISheet>
{
    Transform Container { get; }
    bool IsGlobal { get; }
}