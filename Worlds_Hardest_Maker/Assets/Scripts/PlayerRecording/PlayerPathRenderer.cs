using System.Collections;
using System.Collections.Generic;
using LuLib.Color;
using LuLib.Transform;
using UnityEngine;

public class PlayerPathRenderer : RecordingRenderer
{
    private const float VALUE_SHIFT = 0.3090169945f;
    
    private readonly Transform recordingPathContainer;
    private readonly Color successColor;
    private readonly Color deathColor;
    private readonly float minDeathColorValue;

    private LineRenderer lineRenderer;
    private Color lineColor;

    private List<RecordingFrame> recordedPositions => FrameProvider.RecordedPositions;

    public PlayerPathRenderer(
        IRecordingFrameProvider frameProvider, 
        Transform recordingPathContainer, 
        Color successColor, Color deathColor,
        float minDeathColorValue) : base(frameProvider)
    {
        this.recordingPathContainer = recordingPathContainer;
        this.successColor = successColor;
        this.deathColor = deathColor;
        this.minDeathColorValue = minDeathColorValue;
    }

    public IEnumerator RenderPathRecording()
    {
        recordingPathContainer.DestroyChildren();
        
        BeginNewLine();
        
        lineRenderer.startColor = deathColor;
        lineRenderer.endColor = deathColor;

        return RenderLoop(i =>
            {
                RecordingFrame currentFrame = recordedPositions[i];
                
                // display line
                AddLinePosition(currentFrame.Position);

                // if player dies or hits checkpoint and then will die, begin new red line 
                if (currentFrame.Died || (currentFrame.CheckpointHit && !currentFrame.StartSuccessfulLine))
                {
                    if (currentFrame.Died)
                    {
                        Instantiate(recordingDeathPrefab, recordedPositions[i].Position, Quaternion.identity,
                            recordingPathContainer);
                    }

                    // calculate new color
                    float value = 1;

                    if (minDeathColorValue < 1)
                    {
                        value = (lineRenderer.startColor.GetHSV().z + VALUE_SHIFT) % (1 - minDeathColorValue) +
                                minDeathColorValue;
                    }

                    Color newColor = Color.red.SetValue(value);

                    newColor.a = deathColor.a;

                    BeginNewLine();

                    lineRenderer.startColor = newColor;
                    lineRenderer.endColor = newColor;

                    if (recordedPositions[i].CheckpointHit) AddLinePosition(recordedPositions[i].Position);
                }

                // change color to green when successful run starts
                if (recordedPositions[i].StartSuccessfulLine && !recordedPositions[i].CheckpointHit)
                {
                    BeginNewLine();

                    lineRenderer.startColor = successColor;
                    lineRenderer.endColor = successColor;
                }

                eventBus.Fire(new RecordingPathRenderUpdateEvent(recordedPositions[i].Position));

                if (IsReplaying && i == recordedPositions.Count - 1)
                {
                    eventBus.Fire(new FinishReplayEvent());
                }
            }
        );
    }
    
    private void AddLinePosition(Vector2 position)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
    }
    
    private void BeginNewLine() => lineRenderer = Instantiate(recordingLinePrefab, recordingPathContainer);
}