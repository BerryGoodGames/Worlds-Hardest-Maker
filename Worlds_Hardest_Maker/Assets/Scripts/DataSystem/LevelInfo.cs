using System;
using UnityEngine.Serialization;

[Serializable]
public class LevelInfo
{
    public string Description = "Default description";
    public DateTime LastEdited = DateTime.Now;
    public TimeSpan EditTime = TimeSpan.Zero;
    public TimeSpan PlayTime = TimeSpan.Zero;
    public TimeSpan BestCompletionTime = TimeSpan.Zero;
    public string Creator = "Default creator name";
    public uint Deaths;
    public uint Completions;
}