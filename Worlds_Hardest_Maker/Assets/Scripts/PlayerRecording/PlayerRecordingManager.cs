using MyBox;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.PlayerRecording.Recording;
using WorldsHardestMaker.PlayerRecording.Rendering;

namespace WorldsHardestMaker.PlayerRecording
{
    public class PlayerRecordingManager : MonoBehaviour, IRecordingService
    {
        [SerializeField] private PlayerRecordingController recordingController;
        [Separator] [SerializeField] private RenderingController renderingController;

        private VisibilityController visibilityController;
    
        private RecordingAnalyzer analyzer;
    
        private EventBus eventBus;
        [Inject] private IPlayerManager playerManager;
    
        public bool IsReplaying { get; private set; }
    
        [Inject]
        private void Construct(EventBus eventBus)
        {
            this.eventBus = eventBus;
        
            eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
            eventBus.Subscribe<SetupPlaySceneEvent>(OnSwitchToPlay);
            eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
            eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);
            eventBus.Subscribe<ReplayEvent>(OnReplay);
        
            renderingController.SetEventBus(eventBus);
        }
    
        private void Awake()
        {
            visibilityController = new(renderingController, eventBus);
            analyzer = new();
        
            recordingController.Initialize(playerManager);
            recordingController.SetRunner(this);
            renderingController.SetRunner(this);
            renderingController.SetFrameStorages(analyzer, recordingController);
        }

        private void Start()
        {
            renderingController.Initialize();
        }

        private void OnSwitchToPlay(SwitchToPlayEvent evt) => OnSwitchToPlay();
        private void OnSwitchToPlay(SetupPlaySceneEvent evt) => OnSwitchToPlay();
        private void OnSwitchToPlay()
        {
            renderingController.StopAndClearRenderings();
        
            recordingController.StartRecording();
        }
    
        private void OnSwitchToEdit(SwitchToEditEvent evt) => AnalyzeAndRender();
    
        private void AnalyzeAndRender()
        {
            recordingController.StopRecording();

            analyzer.AnalyzeFrames(recordingController);
        
            renderingController.RenderAll();
        }
    
        private void OnPlayAgain(PlayAgainEvent evt)
        {
            recordingController.StartRecording();
        
            visibilityController.SetOnionVisible(false);
            visibilityController.SetPathVisible(false);
        
            IsReplaying = false;
        }
    
        private void OnReplay(ReplayEvent evt)
        {
            visibilityController.SetOnionVisible(false);
            visibilityController.SetPathVisible(true);
        
            IsReplaying = true;
        }
    
        public void SetPathVisible(bool visible)
        {
            visibilityController.SetPathVisible(visible);
        }

        public void SetOnionVisible(bool visible)
        {
            visibilityController.SetOnionVisible(visible);
        }

        public void StopReplay()
        {
            IsReplaying = false;
        }
    
        private void OnDestroy()
        {
            visibilityController.Dispose();
        
            eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
            eventBus.Unsubscribe<SetupPlaySceneEvent>(OnSwitchToPlay);
            eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
            eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
            eventBus.Unsubscribe<ReplayEvent>(OnReplay);
        }
    }
}