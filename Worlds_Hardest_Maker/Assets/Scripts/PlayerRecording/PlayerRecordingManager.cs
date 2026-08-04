using MyBox;
using UnityEngine;
using VContainer;

// TODO: split into RecordingController, RecordingRenderer, RecordingVisibilityController
public class PlayerRecordingManager : MonoBehaviour
{
    public static PlayerRecordingManager Instance { get; private set; }
    
    [SerializeField] private PlayerRecorder playerRecorder;
    
    [Separator] [SerializeField] private PlayerPathRenderer pathRenderer;    
    [SerializeField] [InitializationField] [OverrideLabel("Display path at start")] private bool displayPath = true;
    
    [Separator] [SerializeField] private PlayerGhostRenderer<RawRecordingFrame> ghostRenderer;
    [SerializeField] [InitializationField] [OverrideLabel("Display sprites at start")] private bool displaySprites = true;

    [Separator] [SerializeField] private RecordingRenderLoop renderLoop;
    
    public bool IsReplaying { get; set; }
    
    private RecordingAnalyzer analyzer;
    
    private Coroutine recording;
    private Coroutine displaySpriteRecording;
    private Coroutine displayPathRecording;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SetupPlaySceneEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);
        eventBus.Subscribe<ReplayEvent>(OnReplay);
        eventBus.Subscribe<TogglePlayerRecordingPathVisibilityRequest>(OnTogglePathVisibility);
        eventBus.Subscribe<TogglePlayerRecordingSpriteVisibilityRequest>(OnToggleSpriteVisibility);
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        analyzer = new();
        pathRenderer.SetFrameStorage(analyzer);
        ghostRenderer.SetFrameStorage(playerRecorder);
    }
    
    private void Start()
    {
        pathRenderer.SetActive(displayPath);
        ghostRenderer.SetActive(displaySprites);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => OnSwitchToPlay();
    private void OnSwitchToPlay(SetupPlaySceneEvent evt) => OnSwitchToPlay();
    private void OnSwitchToPlay()
    {
        if (displaySpriteRecording != null) StopCoroutine(displaySpriteRecording);
        if (displayPathRecording != null) StopCoroutine(displayPathRecording);
        
        pathRenderer.Clear();
        ghostRenderer.Clear();
        
        recording = StartCoroutine(playerRecorder.RecordPlayer());
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt) => AnalyzeAndRender();
    
    private void AnalyzeAndRender()
    {
        if (recording != null) StopCoroutine(recording);

        analyzer.AnalyzeFrames(playerRecorder);

        if (ghostRenderer.IsActive())
        {
            displaySpriteRecording = StartCoroutine(ghostRenderer.RenderSpriteRecording(renderLoop));
        }
        if (pathRenderer.IsActive())
        {
            displayPathRecording = StartCoroutine(pathRenderer.RenderPathRecording(renderLoop));
        }
    }
    
    private void StartPlayerRecording()
    {
        if (recording != null) StopCoroutine(recording);
        
        recording = StartCoroutine(playerRecorder.RecordPlayer());
    }
    
    private void OnToggleSpriteVisibility(TogglePlayerRecordingSpriteVisibilityRequest req)
    {
        SetSpriteVisible(!ghostRenderer.IsActive());
    }

    public void SetSpriteVisible(bool visible)
    {
        ghostRenderer.SetActive(visible);
        
        if (visible) displaySpriteRecording = StartCoroutine(ghostRenderer.RenderSpriteRecording(renderLoop));
        else
        {
            if (displaySpriteRecording != null) StopCoroutine(displaySpriteRecording);
            ghostRenderer.Clear();
        }
    }
    
    private void OnTogglePathVisibility(TogglePlayerRecordingPathVisibilityRequest req)
    {
        SetPathVisible(!pathRenderer.IsActive());
    }

    public void SetPathVisible(bool visible)
    {
        pathRenderer.SetActive(visible);
        
        if (displayPathRecording != null) StopCoroutine(displayPathRecording);
        
        pathRenderer.Clear();
        
        if (visible) displayPathRecording = StartCoroutine(pathRenderer.RenderPathRecording(renderLoop));
    }
    
    private void OnPlayAgain(PlayAgainEvent evt)
    {
        StartPlayerRecording();
        
        SetSpriteVisible(false);
        SetPathVisible(false);
        
        IsReplaying = false;
    }
    
    private void OnReplay(ReplayEvent evt)
    {
        SetSpriteVisible(false);
        SetPathVisible(true);
        
        IsReplaying = true;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SetupPlaySceneEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
        eventBus.Unsubscribe<ReplayEvent>(OnReplay);
        eventBus.Unsubscribe<TogglePlayerRecordingPathVisibilityRequest>(OnTogglePathVisibility);
        eventBus.Unsubscribe<TogglePlayerRecordingSpriteVisibilityRequest>(OnToggleSpriteVisibility);
    }
}