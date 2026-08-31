using UnityEngine;

public class PlayerQueryService : ILevelObjectQuery<PlayerController>
{
    private readonly IPlayerManager playerManager;

    public PlayerQueryService(IPlayerManager playerManager)
    {
        this.playerManager = playerManager;
    }
    
    public PlayerController Find(Vector2 position, ISheet sheet)
    {
        return Exists(position, sheet) ? playerManager.Player : null;
    }

    public bool Exists(Vector2 position, ISheet sheet)
    {
        PlayerController player = playerManager.Player;
        if (player == null) return false;
        if (player.Sheet != sheet) return false;
        if ((Vector2)player.transform.position != position) return false;
        return true;
    }
}