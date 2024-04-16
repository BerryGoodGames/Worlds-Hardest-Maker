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

    public GameState(Vector2 playerStartPos, List<Vector2> collectedCoins, List<Vector2> collectedKeys)
    {
        // PlayerStartPos = playerStartPos;
        CollectedCoins = collectedCoins;
        CollectedKeys = collectedKeys;
    }
}