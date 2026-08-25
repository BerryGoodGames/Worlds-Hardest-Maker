using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerFactory : ILevelObjectFactory<PlayerController>
{
    private readonly IObjectResolver diContainer;
    
    private PlayerController playerPrefab;
    private JumpToEntity mainCameraJumper;
    private TimerController timerController;
    private Transform playerContainer;

    public PlayerFactory(IObjectResolver diContainer)
    {
        this.diContainer = diContainer;
    }
    
    public void Initialize(PlayerController playerPrefab, JumpToEntity mainCameraJumper, TimerController timerController, Transform playerContainer)
    {
        this.playerPrefab = playerPrefab;
        this.mainCameraJumper = mainCameraJumper;
        this.timerController = timerController;
        this.playerContainer = playerContainer;
    }
    
    public PlayerController Create(ManagerParameters args)
    {
        PlayerController newPlayer = Object.Instantiate(
            playerPrefab,
            args.Position, Quaternion.identity,
            playerContainer
        );
        
        diContainer.InjectGameObject(newPlayer.gameObject);
        
        newPlayer.Initialize(mainCameraJumper, timerController, playerContainer);
        
        PlaceManager.Instance.AttachToSheet(newPlayer.gameObject, args.Sheet, false);
        newPlayer.Sheet = args.Sheet;
        
        return newPlayer;
    }
}