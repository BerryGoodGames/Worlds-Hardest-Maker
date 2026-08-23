using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;
using VContainer.Unity;

public partial class AnchorController
{
    [SerializeField] [InitializationField] [MustBeAssigned] private AnchorPathLine anchorPathLinePrefab;
    [Separator("Path settings")] [SerializeField] private Transform lineContainer;
    
    [SerializeField] private Color lineColor;
    [SerializeField] private float lineWeight;

    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt)
    {
        if(AnchorManager.Instance.SelectedAnchor == this)
        {
            RenderLines();
        }
    }

    public void RenderLines() => StartCoroutine(RenderLinesCoroutine());

    private IEnumerator RenderLinesCoroutine()
    {
        yield return new WaitForEndOfFrame();
        
        ClearLines();
        
        // line settings
        drawService.SetFill(lineColor);
        drawService.SetLayerID(spriteRenderer.sortingLayerID);
        drawService.SetOrderInLayer(spriteRenderer.sortingOrder - 1);
        drawService.SetRoundedCorners(true);
        drawService.SetWeight(lineWeight);
        
        Vector2 previousVertex = transform.position;
        
        int index = 0;
        LinkedListNode<AnchorBlock> currentNode = Blocks.First;
        
        bool[] hasRendered = new bool[Blocks.Count];
        
        List<(Vector2, Vector2)> lineList = new();
        
        int loopIndex = -1;
        bool hasLooped = false;
        bool isFirstPositionBlockAfterLoop = false;
        
        // loop through blocks
        while (currentNode != null)
        {
            AnchorBlock currentBlock = currentNode.Value;
            
            // handle current block
            ParseBlockForPath(ref currentBlock, index, hasRendered, lineList, ref previousVertex, ref isFirstPositionBlockAfterLoop, ref loopIndex);
            
            // increment
            index++;
            currentNode = currentNode.Next;
            
            // If currentNode reaches the end, and we haven't looped, jump to loopIndex
            if (currentNode != null) continue;
            
            if (loopIndex == -1 || hasLooped) break;
            
            index = loopIndex;
            currentNode = Blocks.NodeAt(index);
            hasLooped = true;
            isFirstPositionBlockAfterLoop = true;
        }
    }
    
    private void ParseBlockForPath(
        ref AnchorBlock anchorBlock, int index, bool[] hasRendered, List<(Vector2, Vector2)> lineList, ref Vector2 previousVertex,
        ref bool isFirstPositionBlockAfterLoop, ref int loopIndex
    )
    {
        if (anchorBlock is PositionAnchorBlock positionAnchorBlock)
        {
            ParsePositionBlockForPath(ref positionAnchorBlock, index, hasRendered, lineList, ref previousVertex,
                ref isFirstPositionBlockAfterLoop);
        }
        // track loop index if LoopBlock
        else if (anchorBlock.TypeID.Equals("Loop"))
        {
            // track loop index
            loopIndex = index;
        }
    }
    
    private void ParsePositionBlockForPath(
        ref PositionAnchorBlock positionAnchorBlock, int index, bool[] hasRendered, List<(Vector2, Vector2)> lineList, ref Vector2 previousVertex,
        ref bool isFirstPositionBlockAfterLoop
    )
    {
        // add new target to array if MoveBlock or MoveAndRotateBlock
        Vector2 currentVertex = positionAnchorBlock.TargetAbsolute;
        
        if (ReferenceManager.Instance.MainChainController.Children[index] is not PositionAnchorBlockController
            controller) throw new("Controller was for some reason not a position block controller, this shouldn't happen");
        
        // setup line, check if line already rendered
        if (!hasRendered[index] || !lineList.Contains((previousVertex, currentVertex)) ||
            isFirstPositionBlockAfterLoop)
            SetupLine(ref positionAnchorBlock, ref controller, ref currentVertex, in previousVertex, lineList, hasRendered, index);
        
        previousVertex = currentVertex;
        
        isFirstPositionBlockAfterLoop = false;
    }
    
    private void SetupLine(
        ref PositionAnchorBlock positionAnchorBlock, ref PositionAnchorBlockController controller, ref Vector2 currentVertex,
        in Vector2 previousVertex, List<(Vector2, Vector2)> lineList, bool[] hasRendered, int index
    )
    {
        AnchorPathLine line = Instantiate(
            anchorPathLinePrefab, Vector2.zero,
            Quaternion.identity, lineContainer
        );
        
        diContainer.InjectGameObject(line.gameObject);
        
        line.CreateArrowHead(previousVertex, currentVertex);
        line.CreateArrowLine(
            previousVertex, currentVertex,
            !positionAnchorBlock.TypeID.Equals("Teleport")
        );
        
        line.CreateBlur();
        
        controller.Lines.Add(line);
        
        lineList.Add((previousVertex, currentVertex));
        hasRendered[index] = true;
    }
    
    private void ClearLines()
    {
        // clear references in blocks
        foreach (AnchorBlock anchorBlock in Blocks)
        {
            if (anchorBlock is not PositionAnchorBlock positionAnchorBlock) continue;
            
            PositionAnchorBlockController controller = positionAnchorBlock.Controller;
            
            // kill all glow tweens
            foreach (AnchorPathLine line in controller.Lines) line.Blur.DOKill();
            
            controller.Lines.Clear();
        }
        
        // clear lines
        foreach (Transform line in lineContainer) Destroy(line.gameObject);
    }
    
    public void SetLinesActive(bool active) => lineContainer.gameObject.SetActive(active);
}