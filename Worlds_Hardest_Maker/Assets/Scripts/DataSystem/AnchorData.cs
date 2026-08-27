using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Anchor attributes: balls (positions), blocks, position
/// </summary>
[Serializable]
public class AnchorData : NonAttachableData
{
    // (list of coordinates)
    private AttachableData[] attachments;
    
    private AnchorBlockData[] blocks;
    
    private readonly float[] position;
    
    public AnchorData(AnchorController controller)
    {
        // init balls and blocks
        SaveAttachments(controller);
        SaveBlocks(controller);
        
        Vector2 controllerPosition = controller.StartPosition;
        
        // init start position
        position = new[]
        {
            controllerPosition.x,
            controllerPosition.y,
        };
    }
    
    #region Saving / loading properties
    
    private void SaveAttachments(AnchorController controller)
    {
        IReadOnlyList<AnchorAttachment> attachments = controller.Attachments;
        this.attachments = new AttachableData[attachments.Count];
        for (int i = 0; i < attachments.Count; i++) this.attachments[i] = (AttachableData)attachments[i].Controller.GetData();
    }
    
    private void SaveBlocks(AnchorController controller)
    {
        // init blocks
        // loop through LinkedList
        blocks = new AnchorBlockData[controller.Blocks.Count];
        int j = 0;
        for (LinkedListNode<AnchorBlock> currentBlockNode = controller.Blocks.First;
             currentBlockNode != null;
             currentBlockNode = currentBlockNode.Next)
        {
            AnchorBlock currentBlock = currentBlockNode.Value;
            
            // assign data
            blocks[j] = currentBlock.GetData();
            
            j++;
        }
    }
    
    private LinkedList<AnchorBlock> LoadBlocks(AnchorController anchor)
    {
        LinkedList<AnchorBlock> blockArr = new();
        
        foreach (AnchorBlockData blockData in blocks)
        {
            AnchorBlock anchorBlock = blockData.GetBlock(anchor);
            blockArr.AddLast(anchorBlock);
        }
        
        return blockArr;
    }
    
    #endregion
    
    public override void ImportToLevel() => ImportToLevel(new(position[0], position[1]));
    
    public override void ImportToLevel(Vector2 pos)
    {
        AnchorController anchor = AnchorManager.Instance.CreateNew(pos, PlaceManager.GetCurrentSheet());
        
        // TODO refactor: creation of new anchor sheet
        foreach (AttachableData data in attachments) data.ImportToLevel(new AnchorSheet(anchor));
        
        if (LevelSessionManager.Instance.IsEdit) AnchorAttachManager.Dehighlight(anchor);
        
        anchor.Blocks = LoadBlocks(anchor);
    }
    
    public override EditMode GetEditMode() => EditModeManager.Anchor;
    
    public override bool Equals(Data d)
    {
        AnchorData other = (AnchorData)d;
        
        if (position[0] != other.position[0] || position[1] != other.position[1]) return false;
        
        if (other.attachments.Length != attachments.Length) return false;
        
        for (int i = 0; i < attachments.Length; i++)
        {
            if (!attachments[i].Equals(other.attachments[i])) return false;
        }
        
        // ignore anchor blocks for now
        return true;
    }
}