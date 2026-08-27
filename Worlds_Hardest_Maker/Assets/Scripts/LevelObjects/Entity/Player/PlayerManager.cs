using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;

public class PlayerManager : MonoBehaviour, ILevelObjectPlacer, ILevelObjectSerializer
{
    public static PlayerManager Instance { get; private set; }
    
    [ReadOnly] public PlayerController Player;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private PlayerController playerPrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private JumpToEntity mainCameraJumper;   
    [SerializeField] [InitializationField] [MustBeAssigned] private TimerController timerController;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform playerContainer;    
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform coinContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform keyContainer;
    
    [Inject] private IObjectResolver diContainer;
    [Inject] private IPositionQueryService positionQueryService;
    [Inject] private ILevelObjectQuery<PlayerController> playerQueryService;
    [Inject] private PlayerPlacementRules placementRules;
    private PlayerFactory playerFactory;

    [Inject]
    private void Construct(PlayerFactory playerFactory)
    {
        this.playerFactory = playerFactory;
        playerFactory.Initialize(playerPrefab, mainCameraJumper, timerController, playerContainer);
    }
    
    public PlayerController CreateNew(Vector2 position, ISheet sheet, bool surroundWithStartFields)
    {
        if (playerQueryService.Exists(position, sheet)) return null;
        
        bool canPlaceInSheet = placementRules.CanPlaceInSheet(position, sheet);
        
        if (surroundWithStartFields && !canPlaceInSheet) SetSurroundingStartFieldsInSheet(position, sheet);
        
        // clear area from coins and keys
        GameManager.Instance.RemoveObjectInContainer(position, coinContainer);
        GameManager.Instance.RemoveObjectInContainer(position, keyContainer);
        
        // if player already exists, just move it
        if (Player != null) Player.ReSet(position, sheet);
        else
        {
            // place player
            PlayerController newPlayer = playerFactory.Create(position, sheet);
            
            // set target of camera
            mainCameraJumper.SetTarget("Player", newPlayer.gameObject);
            
            Player = newPlayer;
        }
        
        return Player;
    }
    
    public PlayerController Set(Vector2 position)
    {
        return CreateNew(position, PlaceManager.GetCurrentSheet(), true);
    }
    
    private static List<FieldController> SetSurroundingStartFieldsInSheet(Vector2 position, ISheet sheet)
    {
        List<FieldController> result = new();
        
        Vector2Int[] checkPoses =
        {
            Vector2Int.FloorToInt(position),
            new(Mathf.CeilToInt(position.x), Mathf.FloorToInt(position.y)),
            new(Mathf.FloorToInt(position.x), Mathf.CeilToInt(position.y)),
            Vector2Int.CeilToInt(position),
        };
        
        foreach (Vector2Int checkPosition in checkPoses)
        {
            FieldController newField = FieldManager.Instance.CreateNew(checkPosition, 0, sheet, EditModeManager.Start);
            
            result.Add(newField);
        }
        
        return result;
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
        PlayerController player = positionQueryService.QueryPosition<PlayerController>(position, 0.1f, LayerManager.Instance.Layers.Player, "PlayerCenterCollider", sheet);
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
            bool isThere = Player != null && (Vector2)Player.transform.position == position + d;
            if (isThere) return true;
        }
        
        return false;
    }
    
    public static Vector2Int GetCurrentRoom() => Instance.Player != null ? Instance.Player.GetCurrentRoom() : Vector2Int.zero;
    public static Vector2Int GetStartRoom() => Instance.Player != null ? Instance.Player.GetStartRoom() : Vector2Int.zero;
    
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

        PlayerController result = CreateNew(gridPosition, PlaceManager.GetCurrentSheet(), true);

        return PlacementResult.FromController(result);
    }

    public IEnumerable<Data> Serialize()
    {
        List<Data> levelData = new();
        if (Player == null || Player.IsAttached) return levelData;
        
        PlayerData playerData = new(Player);
        levelData.Add(playerData);
        
        return levelData;
    }
}