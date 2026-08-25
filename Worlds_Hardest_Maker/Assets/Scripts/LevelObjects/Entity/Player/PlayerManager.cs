using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerManager : MonoBehaviour, 
    IManager<PlayerController>, 
    IManagerPlaceRestrictable, 
    ILevelObjectManager
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
    
    public PlayerController SetInSheet(ManagerParameters args)
    {
        Vector2 position = args.Position;
        
        if (IsThereInSheet(position, args.Sheet)) return null;
        
        bool canPlaceInSheet = CanPlaceInSheet(position, args.Sheet);
        
        if (args.SurroundWithStartFields && !canPlaceInSheet) SetSurroundingStartFieldsInSheet(position, args.Sheet);
        
        // clear area from coins and keys
        GameManager.Instance.RemoveObjectInContainer(position, coinContainer);
        GameManager.Instance.RemoveObjectInContainer(position, keyContainer);
        
        // if player already exists, just move it
        if (Player != null) Player.ReSet(args);
        else
        {
            // place player
            PlayerController newPlayer = ((IManager<PlayerController>)this).InstantiateInSheet(args);
            
            // set target of camera
            mainCameraJumper.SetTarget("Player", newPlayer.gameObject);
            
            Player = newPlayer;
        }
        
        return Player;
    }
    
    public PlayerController Set(Vector2 position)
    {
        ManagerParameters args = new() { Position = position, SurroundWithStartFields = true, };
        return ((IManager<PlayerController>)this).Set(args);
    }
    
    public PlayerController GetInSheet(Vector2 position, AnchorController sheet) => IsThereInSheet(position, sheet) ? Player : null;
    
    public PlayerController InstantiateInSheet(ManagerParameters args)
    {
        PlayerController newPlayer = Instantiate(
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
    
    public bool IsThere(Vector2 position) => Instance.Player != null && (Vector2)Instance.Player.transform.position == position;
    public bool IsThereInSheet(Vector2 position, AnchorController sheet) => IsThere(position) && Instance.Player.Sheet == sheet;
    
    public bool CanPlace(Vector2 position)
    {
        print(IsThere(position));
        print(
            FieldManager.Instance.IsPosCoveredWithFieldTypeInSheet(
                position, PlaceManager.GetCurrentSheet(), EditModeManager.Instance.AllPlayerStartFieldModes.ToArray()
            )
        );
        
        // conditions: no player there, position is covered with possible start fields
        return !IsThere(position) &&
               FieldManager.Instance.IsPosCoveredWithFieldTypeInSheet(
                   position, PlaceManager.GetCurrentSheet(), EditModeManager.Instance.AllPlayerStartFieldModes.ToArray()
               );
    }
    
    public bool CanPlaceInSheet(Vector2 position, AnchorController sheet) =>
        // conditions: no player there, position is covered with possible start fields
        !IsThereInSheet(position, sheet) &&
        FieldManager.Instance.IsPosCoveredWithFieldTypeInSheet(position, sheet, EditModeManager.Instance.AllPlayerStartFieldModes.ToArray());
    
    private static List<FieldController> SetSurroundingStartFieldsInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
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
            ManagerParameters args = new()
            {
                Position = checkPosition,
                FieldMode = EditModeManager.Start,
                Sheet = sheet,
            };
            
            result.Add(((IManager<FieldController>)FieldManager.Instance).SetInSheet(args));
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
    
    private void RemoveAtPositionInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
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
    
    public void RemoveAtPosIntersectInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
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
            if (IsThere(position + d)) return true;
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

    public bool Place(PlacementRequest request)
    {
        Vector2 gridPosition = request.Position.ConvertToGrid();
        
        ManagerParameters args = ManagerParameters.FromCurrentSheet(new()
        {
            Position = gridPosition, 
            SurroundWithStartFields = true,
        });
        
        return SetInSheet(args);
    }

    public LevelObjectController Query(Vector2 position, AnchorController sheet)
    {
        return GetInSheet(position, sheet);
    }

    public bool Remove(Vector2 position, AnchorController sheet)
    {
        bool existed = GetInSheet(position, sheet) != null;
        RemoveAtPositionInSheet(position, sheet);
        return existed;
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