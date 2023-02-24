using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Saves progress of player in current playing session
/// </summary>
public class GameState
{
    public Vector2 PlayerStartPos;
    public List<Vector2> CollectedCoins;
    public List<Vector2> CollectedKeys;

    public GameState(Vector2 playerStartPos, List<Vector2> collectedCoins, List<Vector2> collectedKeys)
    {
        PlayerStartPos = playerStartPos;
        CollectedCoins = collectedCoins;
        CollectedKeys = collectedKeys;
    }

    public bool Equals(GameState other)
    {
        return PlayerStartPos == other.PlayerStartPos &&
               CollectedCoins.Equals(other.CollectedCoins) &&
               CollectedKeys.Equals(other.CollectedKeys);
    }
}