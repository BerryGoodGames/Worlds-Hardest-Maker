using System.Collections;
using System.Collections.Generic;
using LuLib.Transform;
using MyBox;
using UnityEngine;

public class PlayerGhostRenderer
{
    private readonly IRecordingFrameStorage frameStorage;
    private readonly Transform recordingSpriteContainer;
    private uint spriteFrequency = 2;
    private float spriteMaxAlpha = 0.5f;
    private uint spriteAmount = 9;
    private readonly SpriteRenderer playerPrefabSprite;

    public PlayerGhostRenderer(
        IRecordingFrameStorage frameStorage,
        Transform recordingSpriteContainer,
        uint spriteFrequency,
        float spriteMaxAlpha,
        uint spriteAmount,
        SpriteRenderer playerPrefabSprite)
    {
        this.frameStorage = frameStorage;
        this.recordingSpriteContainer = recordingSpriteContainer;
        this.spriteFrequency = spriteFrequency;
        this.spriteMaxAlpha = spriteMaxAlpha;
        this.spriteAmount = spriteAmount;
        this.playerPrefabSprite = playerPrefabSprite;
    }
    
    public IEnumerator RenderSpriteRecording(RecordingRenderLoop renderLoop)
    {
        IReadOnlyList<RecordingFrame> recordedPositions = frameStorage.RecordedPositions;
        
        if (recordedPositions == null) return null;
        
        recordingSpriteContainer.DestroyChildren();
        
        int startIndex = (int)Mathf.Max(recordedPositions.Count - spriteAmount * spriteFrequency, 0);

        return renderLoop.Play(
            (positions, i) =>
            {
                // display player sprite
                float playerTrailIndex = (i - (positions.Count - (float)(spriteAmount * spriteFrequency))) /
                    spriteFrequency + 1;

                if (playerTrailIndex <= 0 || (positions.Count - 1 - i) % spriteFrequency != 0) return;

                SpriteRenderer playerTrail = Object.Instantiate(
                    playerPrefabSprite, positions[i].Position, Quaternion.identity, recordingSpriteContainer
                );

                float alpha = playerTrailIndex * spriteMaxAlpha / spriteAmount;
                playerTrail.SetAlpha(alpha);
            }, recordedPositions, startIndex
        );
    }
}