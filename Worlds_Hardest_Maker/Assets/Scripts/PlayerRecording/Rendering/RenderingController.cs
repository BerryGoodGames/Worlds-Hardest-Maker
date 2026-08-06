using System;
using MyBox;
using UnityEngine;
using WorldsHardestMaker.PlayerRecording.Recording;

namespace WorldsHardestMaker.PlayerRecording.Rendering
{
    [Serializable]
    public class RenderingController
    {
        [SerializeField] private PathRenderer pathRenderer;    
        [SerializeField] [InitializationField] [OverrideLabel("Display path at start")] private bool displayPath = true;
    
        [Space] [SerializeField] private OnionRenderer<RawFrame> onionRenderer;
        [SerializeField] [InitializationField] [OverrideLabel("Display onion at start")] private bool displayOnion = true;

        [Space] [SerializeField] private RenderLoop renderLoop;
    
        private Coroutine onionRoutine;
        private Coroutine pathRoutine;
        private MonoBehaviour runner;

        public void SetRunner(MonoBehaviour runner)
        {
            this.runner = runner;
        }

        public void SetFrameStorages(IRecordingFrameStorage<AnalyzedFrame> pathFrameStorage, IRecordingFrameStorage<RawFrame> onionFrameStorage)
        {
            pathRenderer.SetFrameStorage(pathFrameStorage);
            onionRenderer.SetFrameStorage(onionFrameStorage);
        }
    
        public void SetEventBus(EventBus eventBus)
        {
            pathRenderer.SetEventBus(eventBus);
        }

        public void Initialize()
        {
            pathRenderer.SetActive(displayPath);
            onionRenderer.SetActive(displayOnion);
        }

        public void StopAndClearRenderings()
        {
            if (onionRoutine != null) runner.StopCoroutine(onionRoutine);
            if (pathRoutine != null) runner.StopCoroutine(pathRoutine);
        
            pathRenderer.Clear();
            onionRenderer.Clear();
        }

        public void RenderAll()
        {
            if (onionRenderer.IsActive())
            {
                RenderOnion();
            }
            if (pathRenderer.IsActive())
            {
                RenderPath();
            }
        }
    
        public void RenderOnion()
        {
            StopOnion();
            onionRoutine = runner.StartCoroutine(onionRenderer.RenderSpriteRecording(renderLoop));
        }

        public void StopOnion()
        {
            if (onionRoutine != null) runner.StopCoroutine(onionRoutine);

            onionRenderer.Clear();
        }

        public void RenderPath()
        {
            StopPath();
            pathRoutine = runner.StartCoroutine(pathRenderer.RenderPathRecording(renderLoop));
        }

        public void StopPath()
        {
            if (pathRoutine != null) runner.StopCoroutine(pathRoutine);

            pathRenderer.Clear();
        }
    
        public bool IsOnionActive()
        {
            return onionRenderer.IsActive();
        }

        public bool IsPathActive()
        {
            return pathRenderer.IsActive();
        }

        public void SetOnionActive(bool active)
        {
            onionRenderer.SetActive(active);
        }

        public void SetPathActive(bool active)
        {
            pathRenderer.SetActive(active);
        }
    }
}