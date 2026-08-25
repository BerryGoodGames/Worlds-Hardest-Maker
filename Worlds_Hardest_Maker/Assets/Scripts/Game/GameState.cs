using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Saves progress of player in current playing session
/// </summary>
public class GameState
{
    public CheckpointController Checkpoint;
    // TODO: should these be positions? CoinControllers make more sense
    public List<Vector2> CollectedCoins;
    public List<Vector2> CollectedKeys;
}