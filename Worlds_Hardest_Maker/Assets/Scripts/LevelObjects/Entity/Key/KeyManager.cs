using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class KeyManager : MonoBehaviour, IManager<KeyController>, IManagerPlaceRestrictable
{
    public static KeyManager Instance { get; private set; }
    
    private IObjectResolver IObjectResolver;
    
    public Transform DefaultContainer => ReferenceManager.Instance.KeyContainer;
    
    [UsedImplicitly] public static readonly List<FieldMode> CannotPlaceFields = new();
    
    private static readonly int playing = Animator.StringToHash("Playing");
    
    [ReadOnly] public List<KeyController> Keys = new();
    [ReadOnly] public List<KeyController> CollectedKeys = new();
    
    private IKonamiService konamiService;
    
    [Inject]
    private void Construct(IObjectResolver IObjectResolver, IKonamiService konamiService)
    {
        this.IObjectResolver = IObjectResolver;
        this.konamiService = konamiService;
    }
    
    private void RemoveKeyInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        KeyController key = GetInSheet(position, sheet);
        
        if (key == null) return;
        
        // un-cache
        Keys.Remove(key);
        
        // destroy
        DestroyImmediate(key.transform.gameObject);
    }
    
    public KeyController SetInSheet(ManagerParameters args)
    {
        if (!CanPlaceInSheet(args.Position, args.Sheet)) return null;
        
        // remove other key (which has mby other color)
        RemoveKeyInSheet(args.Position, args.Sheet);
        
        KeyController key = InstantiateInSheet(args);
        
        key.Color = args.KeyColor;
        
        // setup idle animation
        key.Animator.SetBool(playing, LevelSessionEditManager.Instance.Playing);
        
        // setup konami code animation
        key.KonamiAnimation.enabled = konamiService.IsKonamiActive;
        
        PlaceManager.Instance.AttachToSheet(key.gameObject, args.Sheet);
        
        return key;
    }
    
    public KeyController GetInSheet(Vector2 position, AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, LayerManager.Instance.Layers.Entity);
        
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Key")) continue;
            if (!hit.TryGetComponent(out KeyController key)) continue;
            if (IManager.IsInSheet(key, sheet)) return key;
        }
        
        return null;
    }
    
    public KeyController Get(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out KeyController controller)) return controller;
        }
        
        return null;
    }
    
    public KeyController InstantiateInSheet(ManagerParameters args)
    {
        KeyController key = Instantiate(
            args.KeyColor.GetPrefabKey(),
            args.Position, Quaternion.identity,
            args.Sheet == null ? DefaultContainer : args.Sheet.AttachmentContainer
        );
        
        IObjectResolver.InjectGameObject(key.gameObject);
        
        return key;
    }
    
    public bool IsThereInSheet(Vector2 position, AnchorController sheet) => GetInSheet(position, sheet) != null;
    
    public List<Data> Serialize(List<Data> levelData)
    {
        foreach (KeyController key in Keys)
        {
            if (key.IsAttached) continue;
            
            KeyData keyData = new(key);
            levelData.Add(keyData);
        }
        
        return levelData;
    }
    
    public bool CanPlace(Vector2 position) => CanPlaceInSheet(position, PlaceManager.GetCurrentSheet());
    
    public bool CanPlaceInSheet(Vector2 position, AnchorController sheet) =>
        // conditions: no key there, covered by canplacefield or default, no player there
        !PlayerManager.Instance.IsThere(position)
        && !IsThereInSheet(position, sheet)
        && !FieldManager.Instance.IntersectingAnyFieldsAtPos(position, sheet, CannotPlaceFields.ToArray());
    
    public bool AllKeysCollected(KeyColor color)
    {
        // check if every key of specific color is picked up
        foreach (KeyController key in Keys)
        {
            if (!key.Collected && key.Color == color) return false;
        }
        
        return true;
    }
    
    private void OnPlayAgain() => CollectedKeys.Clear();
    
    private void Start() => LevelCompleteManager.Instance.OnPlayAgain += OnPlayAgain;
    
    private void OnDestroy() => LevelCompleteManager.Instance.OnPlayAgain -= OnPlayAgain;
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
    
    public void ActivateAnimations() => Keys.ForEach(key => key.ActivateAnimation());
    public bool CorrespondsToEditMode(EditMode compare) => compare.Attributes.IsKey;
}