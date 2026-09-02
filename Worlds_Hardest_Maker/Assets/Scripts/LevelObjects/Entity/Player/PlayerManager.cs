using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

public class PlayerManager : MonoBehaviour, 
    IPlayerManager, 
    ILevelObjectPlacer, 
    ILevelObjectSerializer
{
    public static PlayerManager Instance { get; private set; }
    
    [SerializeField] [InitializationField] [MustBeAssigned] private PlayerController playerPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private JumpToEntity mainCameraJumper;   
    [SerializeField] [InitializationField] [MustBeAssigned] private TimerController timerController;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform playerContainer;    
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform coinContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform keyContainer;

    [Inject] private IObjectResolver diContainer;
    private EventBus eventBus;
    [Inject] private IPositionQueryService positionQueryService;
    [Inject] private ILevelObjectQuery<PlayerController> playerQueryService;
    [Inject] private PlayerPlacementRules placementRules;
    private PlayerFactory playerFactory;
    [Inject] private IPlayerProvider playerProvider;

    [Inject]
    private void Construct(EventBus eventBus, PlayerFactory playerFactory)
    {
        this.eventBus = eventBus;
        eventBus.Subscribe<ResetLevelEvent>(OnResetLevel);
        
        this.playerFactory = playerFactory;
        playerFactory.Initialize(playerPrefab, mainCameraJumper, timerController, playerContainer);
    }

    private void OnResetLevel(ResetLevelEvent evt)
    {
        if (playerProvider.HasPlayer) playerProvider.Player.Setup();
    }

    private void OnDestroy()
    {
        eventBus.Unsubscribe<ResetLevelEvent>(OnResetLevel);
    }

    public PlayerController CreateNew(Vector2 position, ISheet sheet, bool surroundWithStartFields)
    {
        if (playerQueryService.Exists(position, sheet)) return null;
        
        if (surroundWithStartFields)
        {
            diContainer.Resolve<PlayerStartFieldResolver>().OnPlayerPlaced(position, sheet);
        }
        
        // clear area from coins and keys
        GameManager.Instance.RemoveObjectInContainer(position, coinContainer);
        GameManager.Instance.RemoveObjectInContainer(position, keyContainer);
        
        // if player already exists, just move it
        if (playerProvider.HasPlayer) playerProvider.Player.ReSet(position, sheet);
        else
        {
            // place player
            PlayerController newPlayer = playerFactory.Create(position, sheet);
            
            // set target of camera
            mainCameraJumper.SetTarget("Player", newPlayer.gameObject);
            
            playerProvider.SetPlayer(newPlayer);
        }
        
        return playerProvider.Player;
    }
    
    public PlayerController Set(Vector2 position)
    {
        return CreateNew(position, PlaceManager.Instance.GetCurrentSheet(), true);
    }
    
    public void RemoveAtPos(Vector2 position)
    {
        // remove player only if at pos
        foreach (Transform player in playerContainer)
        {
            if ((Vector2)player.position == position) player.GetComponent<PlayerController>().DestroySelf();
        }
    }
    
    private void RemoveAtPositionInSheet(Vector2 position, ISheet sheet)
    {
        PlayerController player = positionQueryService.QueryPosition<PlayerController>(
            position, 0.1f, 
            LayerManager.Instance.Layers.Player, 
            "PlayerCenterCollider", 
            sheet,
            SheetUtils.SheetCheckingScope.Parent,
            IPositionQueryService.ComponentCheckingScope.Parent);
        player?.DestroySelf();
    }
    
    public void RemoveAtPosIntersect(Vector2 position)
    {
        Vector2[] deltas =
        {
            new(-0.5f, -0.5f), new(0, -0.5f), new(0.5f, -0.5f),
            new(-0.5f, 0), new(0, 0), new(0.5f, 0),
            new(-0.5f, 0.5f), new(0, 0.5f), new(0.5f, 0.5f),
        };
        
        foreach (Vector2 d in deltas) RemoveAtPos(position + d);
    }
    
    public void RemoveAtPosIntersectInSheet(Vector2 position, ISheet sheet)
    {
        Vector2[] deltas =
        {
            new(-0.5f, -0.5f), new(0, -0.5f), new(0.5f, -0.5f),
            new(-0.5f, 0), new(0, 0), new(0.5f, 0),
            new(-0.5f, 0.5f), new(0, 0.5f), new(0.5f, 0.5f),
        };
        
        foreach (Vector2 d in deltas) RemoveAtPositionInSheet(position + d, sheet);
    }
    
    public bool IsThereIntersect(Vector2 position)
    {
        Vector2[] deltas =
        {
            new(-0.5f, -0.5f), new(0, -0.5f), new(0.5f, -0.5f),
            new(-0.5f, 0), new(0, 0), new(0.5f, 0),
            new(-0.5f, 0.5f), new(0, 0.5f), new(0.5f, 0.5f),
        };
        
        foreach (Vector2 d in deltas)
        {
            bool isThere = playerProvider.HasPlayer && (Vector2)playerProvider.Player.transform.position == position + d;
            if (isThere) return true;
        }
        
        return false;
    }
    
    public Vector2Int GetCurrentRoom()
    {
        return playerProvider.HasPlayer ? playerProvider.Player.GetCurrentRoom() : Vector2Int.zero;
    }

    public Vector2Int GetStartRoom()
    {
        return playerProvider.HasPlayer ? playerProvider.Player.GetStartRoom() : Vector2Int.zero;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
    
    public bool CanHandle(EditMode editMode)
    {
        return editMode == EditModeManager.Player;
    }

    public PlacementResult Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();

        PlayerController result = CreateNew(gridPosition, PlaceManager.Instance.GetCurrentSheet(), true);

        return PlacementResult.FromController(result);
    }

    public IEnumerable<Data> Serialize()
    {
        List<Data> levelData = new();
        if (!playerProvider.HasPlayer || playerProvider.Player.IsAttached) return levelData;
        
        PlayerData playerData = new(playerProvider.Player);
        levelData.Add(playerData);
        
        return levelData;
    }
}