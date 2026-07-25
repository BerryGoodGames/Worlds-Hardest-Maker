using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PreviewSpriteComponent : MonoBehaviour, IPreviewDataReceiver
{
    [Space] [SerializeField] [Range(0, 255)] private float alpha;
    
    public bool ShowSpriteWhenPasting { get; set; }
    
    public SpriteRenderer SpriteRenderer { get; private set; }

    [ReadOnly] public bool CheckUpdateEveryFrame = true;
    
    private EventBus eventBus;
    private IEditModePreviewProvider previewProvider;
    
    [Inject]
    private void Construct(EventBus eventBus, IEditModePreviewProvider previewProvider)
    {
        this.eventBus = eventBus;
        this.previewProvider = previewProvider;
        
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChange);
    }

    public float GetAlpha() => alpha / 255f;
    public void SetAlpha(float newAlpha) => alpha = newAlpha * 255f;
    
    private void OnSwitchToEdit(SwitchToEditEvent evt) => UpdateSprite();
    private void OnEditModeChange(EditModeChangeEvent evt) => UpdateSprite();

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        
        // set the default preview at start
        ApplyPreviewData(PreviewData.Default);
    }
    
    private void Update()
    {
        // Update loop is handled by PreviewCoordinator via events
    }
    
    /// <summary>
    ///     Updates sprite of preview to the current edit mode
    /// </summary>
    public void UpdateSprite()
    {
        SetSprite(LevelSessionEditManager.Instance.CurrentEditMode);
        
        // previousPlaying = LevelSessionEditManager.Instance.Playing;
        // previousEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
    }
    
    public void SetSprite(EditMode editMode)
    {
        float normalizedAlpha = alpha / 255f;
        PreviewData previewData = previewProvider.GetPreviewData(editMode, normalizedAlpha, forceShowPreviewSprite: ShowSpriteWhenPasting);
        
        ApplyPreviewData(previewData);
    }
    
    public void ApplyPreviewData(PreviewData previewData)
    {
        SpriteRenderer.sprite = previewData.Sprite;
        SpriteRenderer.color = previewData.Color;
        transform.localScale = previewData.Scale;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChange);
    }
}