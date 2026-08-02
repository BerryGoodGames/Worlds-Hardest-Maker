using System.Collections;
using System.Collections.Generic;
using LuLib.Transform;
using MyBox;
using UnityEngine;

public class PlayerGhostRenderer : RecordingRenderer
{
    private readonly Transform recordingSpriteContainer;
    private uint spriteFrequency = 2;
    private float spriteMaxAlpha = 0.5f;
    private uint spriteAmount = 9;
    private readonly SpriteRenderer playerPrefabSprite;

    public PlayerGhostRenderer(
        IRecordingFrameProvider frameProvider,
        Transform recordingSpriteContainer,
        uint spriteFrequency,
        float spriteMaxAlpha,
        uint spriteAmount,
        SpriteRenderer playerPrefabSprite) : base(frameProvider)
    {
        this.recordingSpriteContainer = recordingSpriteContainer;
        this.spriteFrequency = spriteFrequency;
        this.spriteMaxAlpha = spriteMaxAlpha;
        this.spriteAmount = spriteAmount;
        this.playerPrefabSprite = playerPrefabSprite;
    }
    
    public IEnumerator RenderSpriteRecording()
    {
        List<RecordingFrame> recordedPositions = FrameProvider.RecordedPositions;
        
        if (recordedPositions == null) return null;
        
        recordingSpriteContainer.DestroyChildren();
        
        int startIndex = (int)Mathf.Max(recordedPositions.Count - spriteAmount * spriteFrequency, 0);

        return RenderLoop(
            i =>
            {
                // display player sprite
                float playerTrailIndex = (i - (recordedPositions.Count - (float)(spriteAmount * spriteFrequency))) /
                    spriteFrequency + 1;

                if (playerTrailIndex <= 0 || (recordedPositions.Count - 1 - i) % spriteFrequency != 0) return;

                SpriteRenderer playerTrail = Instantiate(
                    playerPrefabSprite, recordedPositions[i].Position, Quaternion.identity, recordingSpriteContainer
                );

                float alpha = playerTrailIndex * spriteMaxAlpha / spriteAmount;
                playerTrail.SetAlpha(alpha);
            }, startIndex
        );
    }
}