using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[Serializable]
public class PlayerRecorder : IRecordingFrameStorage<RawRecordingFrame>
{
    [SerializeField] [PositiveValueOnly] private float recordingFrequency = 0.05f;
    
    private readonly List<RawRecordingFrame> recordedPositions = new();
    public IReadOnlyList<RawRecordingFrame> Frames => recordedPositions.AsReadOnly();
    
    public IEnumerator RecordPlayer()
    {
        if (PlayerManager.Instance.Player == null) yield return new WaitForEndOfFrame();
        if (PlayerManager.Instance.Player == null) yield break;
        
        PlayerController player = PlayerManager.Instance.Player;
        
        recordedPositions.Clear();
        
        player.OnDeathEnd += RecordDeath;
        player.OnCheckpointEnter += RecordCheckpoint;
        
        // wait until player is out of the death animation
        while (player.InDeathAnim) yield return null;
        
        // save positions of player
        while (!LevelSessionEditManager.Instance.Editing)
        {
            // only record if player has moved
            if (recordedPositions.Count == 0 || (Vector2)player.transform.position != recordedPositions[^1].Position)
            {
                RawRecordingFrame newFrame = new()
                {
                    Position = player.transform.position,
                };
                recordedPositions.Add(newFrame);
            }
            
            yield return new WaitForSeconds(recordingFrequency);
        }
        
        player.OnDeathEnd -= RecordDeath;
        player.OnCheckpointEnter -= RecordCheckpoint;
        yield break;
        
        void RecordDeath()
        {
            if (LevelSessionEditManager.Instance.Editing) return;
            
            RawRecordingFrame newFrame = new()
            {
                Position = player.transform.position,
                Died = true,
            };
            recordedPositions.Add(newFrame);
        }
        
        void RecordCheckpoint()
        {
            if (LevelSessionEditManager.Instance.Editing) return;
            
            RawRecordingFrame newFrame = new()
            {
                Position = player.transform.position,
                CheckpointHit = true,
            };
            recordedPositions.Add(newFrame);
        }
    }
}