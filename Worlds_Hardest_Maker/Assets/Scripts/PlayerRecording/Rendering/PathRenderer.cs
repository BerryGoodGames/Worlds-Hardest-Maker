using System;
using System.Collections;
using System.Collections.Generic;
using LuLib.Color;
using LuLib.Transform;
using MyBox;
using UnityEngine;
using WorldsHardestMaker.PlayerRecording.Recording;
using Object = UnityEngine.Object;

namespace WorldsHardestMaker.PlayerRecording.Rendering
{
    [Serializable]
    public class PathRenderer
    {
        private const float VALUE_SHIFT = 0.3090169945f;
    
        private IRecordingFrameStorage<AnalyzedFrame> frameStorage;
        [SerializeField] private Transform recordingPathContainer;
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color deathColor = Color.red;
        [SerializeField] [OverrideLabel("Min Value")] [Range(0, 1)] private float minDeathColorValue = 0.5f;
        [SerializeField] private GameObject recordingDeathPrefab;
        [SerializeField] private LineRenderer recordingLinePrefab;
    
        private LineRenderer lineRenderer;
        private Color lineColor;
        private EventBus eventBus;

        public void SetFrameStorage(IRecordingFrameStorage<AnalyzedFrame> frameStorage)
        {
            this.frameStorage = frameStorage;
        }
    
        public void SetEventBus(EventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public void SetActive(bool active)
        {
            recordingPathContainer.gameObject.SetActive(active);
        }

        public bool IsActive()
        {
            return recordingPathContainer.gameObject.activeSelf;
        }

        public void Clear()
        {
            recordingPathContainer.DestroyChildren();
        }

        public IEnumerator RenderPathRecording(RenderLoop renderLoop)
        {
            if (frameStorage == null)
            {
                Debug.LogWarning("Frame storage not assigned.");
                yield break;
            }
        
            IReadOnlyList<AnalyzedFrame> frames = frameStorage.Frames;
        
            recordingPathContainer.DestroyChildren();
        
            BeginNewLine();
        
            lineRenderer.startColor = deathColor;
            lineRenderer.endColor = deathColor;

            yield return renderLoop.Play(
                (positions, i) =>
                {
                    AnalyzedFrame currentFrame = positions[i];
                
                    // display line
                    AddLinePosition(currentFrame.Position);

                    // if player dies or hits checkpoint and then will die, begin new red line 
                    if (currentFrame.Died || (currentFrame.CheckpointHit && !currentFrame.StartsSuccessfulRun))
                    {
                        if (currentFrame.Died)
                        {
                            Object.Instantiate(recordingDeathPrefab, currentFrame.Position, Quaternion.identity, recordingPathContainer);
                        }

                        // calculate new color
                        float value = 1;

                        if (minDeathColorValue < 1)
                        {
                            value = (lineRenderer.startColor.GetHSV().z + VALUE_SHIFT) % (1 - minDeathColorValue) + minDeathColorValue;
                        }

                        Color newColor = Color.red.SetValue(value);

                        newColor.a = deathColor.a;

                        BeginNewLine();

                        lineRenderer.startColor = newColor;
                        lineRenderer.endColor = newColor;

                        if (currentFrame.CheckpointHit) AddLinePosition(currentFrame.Position);
                    }

                    // change color to green when successful run starts
                    if (currentFrame.StartsSuccessfulRun && !currentFrame.CheckpointHit)
                    {
                        BeginNewLine();

                        lineRenderer.startColor = successColor;
                        lineRenderer.endColor = successColor;
                    }

                    if (eventBus != null)
                    {
                        eventBus.Fire(new PathRenderUpdateEvent(currentFrame.Position));

                        if (i == positions.Count - 1)
                        {
                            eventBus.Fire(new FinishReplayEvent());
                        }
                    }
                }, frames
            );
        }
    
        private void AddLinePosition(Vector2 position)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
        }
    
        private void BeginNewLine()
        {
            lineRenderer = Object.Instantiate(recordingLinePrefab, recordingPathContainer);
        }
    }
}