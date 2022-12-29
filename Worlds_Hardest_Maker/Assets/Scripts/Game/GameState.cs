using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     class, where progress of player can be saved
/// </summary>
public class GameState
{
    public Vector2 playerStartPos;
    public List<Vector2> collectedCoins;
    public List<Vector2> collectedKeys;

    public GameState(Vector2 playerStartPos, List<Vector2> collectedCoins, List<Vector2> collectedKeys)
    {
        this.playerStartPos = playerStartPos;
        this.collectedCoins = collectedCoins;
        this.collectedKeys = collectedKeys;
    }

    public bool Equals(GameState other)
    {
        return playerStartPos == other.playerStartPos &&
               collectedCoins.Equals(other.collectedCoins) &&
               collectedKeys.Equals(other.collectedKeys);
    }
}