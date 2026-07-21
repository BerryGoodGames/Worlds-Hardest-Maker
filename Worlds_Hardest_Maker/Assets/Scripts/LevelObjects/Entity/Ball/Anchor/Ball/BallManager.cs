using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Zenject;

public class BallManager : MonoBehaviour, IManager<BallController>
{
    public static BallManager Instance { get; private set; }
    
    [ReadOnly] public List<BallController> BallList;
    [ReadOnly] public Dictionary<AnchorController, List<BallController>> BallListSheets;
    [ReadOnly] public List<BallController> BallListGlobal;
    
    private DiContainer diContainer;
    
    [Inject]
    private void Construct(DiContainer diContainer)
    {
        this.diContainer = diContainer;
    }

    #region Set, Get
    
    public Transform DefaultContainer => ReferenceManager.Instance.BallContainer;
    
    public BallController SetInSheet(ManagerParameters args)
    {
        if (GetInSheet(args.Position, args.Sheet) != null) return null;
        
        BallController ballController = InstantiateInSheet(args);
        
        // setup parent
        if (AnchorAttachManager.Instance.InAttachMode)
        {
            ballController.ParentAnchor = args.Sheet;
            args.Sheet.Balls.Add(ballController.LevelObject.transform);
        }
        
        ballController.transform.position = args.Position;
        
        // track ball positions in all the layers
        Instance.BallList.Add(ballController);
        
        if (AnchorAttachManager.Instance.InAttachMode) Instance.BallListSheets[AnchorManager.Instance.SelectedAnchor].Add(ballController);
        else Instance.BallListGlobal.Add(ballController);
        
        PlaceManager.AttachToSheet(ballController.LevelObject, args.Sheet);
        
        return ballController;
    }
    
    public BallController GetInSheet(Vector2 position, AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("BallObject")) continue;
            if (!hit.TryGetComponent(out BallController ball)) continue;
            if (IManager.IsInSheet(ball.transform.parent, sheet)) return ball;
        }
        
        return null;
    }
    
    public BallController InstantiateInSheet(ManagerParameters args)
    {
        Transform container = args.Sheet == null ? DefaultContainer : args.Sheet.AttachmentContainer;
        
        GameObject ball = Instantiate(
            PrefabManager.Instance.Ball,
            args.Position, Quaternion.identity,
            container
        );
        
        diContainer.InjectGameObject(ball);
        
        return ball.GetComponentInChildren<BallController>();
    }
    
    public List<Data> Serialize(List<Data> levelData)
    {
        if (BallListGlobal == null) return levelData;
        foreach (BallController ball in BallListGlobal)
        {
            if (ball.IsAttached) continue;
            
            BallData ballData = (BallData)ball.GetData();
            levelData.Add(ballData);
        }
        
        return levelData;
    }
    
    #endregion
    
    private void Start()
    {
        BallListSheets = new();
        BallListGlobal = new();
    }
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
    
    public bool CorrespondsToEditMode(EditMode compare) => compare == EditModeManager.Ball;
}