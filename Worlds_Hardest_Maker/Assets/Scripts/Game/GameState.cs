using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
///     Saves progress of player in current playing session
/// </summary>
public class GameState
{
    public CheckpointController Checkpoint;
    public List<Vector2> CollectedCoins;
    public List<Vector2> CollectedKeys;
}