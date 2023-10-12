using System;

[Serializable]
public class LevelInfo
{
    public string Description = "Default description";
    public DateTime LastEdited = DateTime.Now;
    public TimeSpan EditTime = TimeSpan.Zero;
    public TimeSpan PlayTime = TimeSpan.Zero;
    public bool Completed;
    public TimeSpan CompleteTime = TimeSpan.Zero;
    public string Creator = "Default creator name";
    public uint Tries;
    public uint Completions;
}
