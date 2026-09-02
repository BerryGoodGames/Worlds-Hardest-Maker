using UnityEngine;

public class PlayerQueryService : ILevelObjectQuery<PlayerController>
{
    private readonly IPlayerProvider playerProvider;

    public PlayerQueryService(IPlayerProvider playerProvider)
    {
        this.playerProvider = playerProvider;
    }
    
    public PlayerController Find(Vector2 position, ISheet sheet)
    {
        return Exists(position, sheet) ? playerProvider.Player : null;
    }

    public bool Exists(Vector2 position, ISheet sheet)
    {
        PlayerController player = playerProvider.Player;
        if (player == null) return false;
        if (player.Sheet != sheet) return false;
        if ((Vector2)player.transform.position != position) return false;
        return true;
    }
}