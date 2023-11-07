using System;

[Serializable]
public class LevelInfo
{
    public string Description = "Unknown";
    public DateTime LastEdited = DateTime.Now;
    public TimeSpan EditTime = TimeSpan.Zero;
    public TimeSpan PlayTime = TimeSpan.Zero;
    public TimeSpan BestCompletionTime = TimeSpan.MaxValue;
    public string Creator = "Unknown";
    public uint Deaths;
    public uint Completions;
}