using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : IRecordingFrameProvider
{
    public List<RecordingFrame> RecordedPositions { get; private set; }
    
    private readonly float recordingFrequency;

    public PlayerRecorder(float recordingFrequency)
    {
        this.recordingFrequency = recordingFrequency;
    }
    
    public IEnumerator RecordPlayer()
    {
        if (PlayerManager.Instance.Player == null) yield return new WaitForEndOfFrame();
        if (PlayerManager.Instance.Player == null) yield break;
        
        PlayerController player = PlayerManager.Instance.Player;
        
        RecordedPositions = new();
        
        player.OnDeathEnd += RecordDeath;
        player.OnCheckpointEnter += RecordCheckpoint;
        
        // wait until player is out of the death animation
        while (player.InDeathAnim) yield return null;
        
        // save positions of player
        while (!LevelSessionEditManager.Instance.Editing)
        {
            // only record if player has moved
            bool hasMovedSinceLastFrame = (Vector2)player.transform.position != RecordedPositions[^1].Position;
            if (RecordedPositions.Count == 0 || hasMovedSinceLastFrame)
            {
                RecordingFrame newFrame = new()
                {
                    Position = player.transform.position,
                };
                RecordedPositions.Add(newFrame);
            }
            
            yield return new WaitForSeconds(recordingFrequency);
        }
        
        player.OnDeathEnd -= RecordDeath;
        player.OnCheckpointEnter -= RecordCheckpoint;
        yield break;
        
        void RecordDeath()
        {
            if (LevelSessionEditManager.Instance.Editing) return;
            
            RecordingFrame newFrame = new()
            {
                Position = player.transform.position,
                Died = true,
            };
            RecordedPositions.Add(newFrame);
        }
        
        void RecordCheckpoint()
        {
            if (LevelSessionEditManager.Instance.Editing) return;
            
            RecordingFrame newFrame = new()
            {
                Position = player.transform.position,
                CheckpointHit = true,
            };
            RecordedPositions.Add(newFrame);
        }
    }
}