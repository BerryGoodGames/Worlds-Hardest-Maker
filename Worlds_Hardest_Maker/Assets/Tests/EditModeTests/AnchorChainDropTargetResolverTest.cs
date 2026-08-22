using System;
using NUnit.Framework;
using UnityEngine;

public class AnchorChainDropTargetResolverTest
{
    private AnchorChainDropTargetResolver resolver;

    [SetUp]
    public void SetUp() => resolver = new AnchorChainDropTargetResolver();

    [Test]
    public void Resolve_PointerOverMiddleBlock_ReturnsInsertAfterThatIndex()
    {
        Rect[] blocks =
        {
            new(0, 0, 100, 50),
            new(0, 50, 100, 50),
            new(0, 100, 100, 50),
        };
        Rect connector = new(0, 150, 100, 20);

        DropTarget result = resolver.Resolve(new(50, 75), blocks, connector);

        Assert.AreEqual(DropTarget.InsertAfter(1), result);
    }

    [Test]
    public void Resolve_PointerOverConnectorRect_ReturnsAppendEnd()
    {
        Rect[] blocks = { new(0, 0, 100, 50) };
        Rect connector = new(0, 50, 100, 20);

        DropTarget result = resolver.Resolve(new(50, 60), blocks, connector);

        Assert.AreEqual(DropTarget.AppendEnd(), result);
    }

    [Test]
    public void Resolve_PointerOutsideEverything_ReturnsNone()
    {
        Rect[] blocks = { new(0, 0, 100, 50) };
        Rect connector = new(0, 50, 100, 20);

        DropTarget result = resolver.Resolve(new(9999, 9999), blocks, connector);

        Assert.AreEqual(DropTarget.None, result);
    }

    [Test]
    public void Resolve_EmptyChain_ReturnsAppendEndRegardlessOfPointer()
    {
        DropTarget result = resolver.Resolve(new(50, 50), Array.Empty<Rect>(), new Rect(0, 0, 100, 20));

        Assert.AreEqual(DropTarget.AppendEnd(), result);
    }

    // This single test would have caught the double-insert risk in section 3.3:
    // with a single resolver and a single call site, there is exactly one
    // documented winner, not "whichever raw MouseOverUIRect.Over flags happened
    // to both be true that frame".
    [Test]
    public void Resolve_PointerOnBoundaryBetweenTwoBlocks_ResolvesToLowerBlockDeterministically()
    {
        Rect[] blocks =
        {
            new(0, 0, 100, 50),   // y in [0, 50)
            new(0, 50, 100, 50),  // y in [50, 100)
        };
        Rect connector = new(0, 100, 100, 20);

        // y = 50 sits exactly on the shared edge.
        DropTarget result = resolver.Resolve(new(50, 50), blocks, connector);

        // Rect.Contains is half-open on the min side, so y=50 belongs to block 1.
        Assert.AreEqual(DropTarget.InsertAfter(1), result);
    }

    [Test]
    public void Resolve_PointerOverlappingBlockAndConnector_BlockWins()
    {
        // Regression guard for the exact section 3.3 scenario: connector and a
        // block region overlap. Block priority must win, not "whichever fired last".
        Rect[] blocks = { new(0, 0, 100, 60) };
        Rect connector = new(0, 40, 100, 30); // overlaps the block from y=40..60

        DropTarget result = resolver.Resolve(new(50, 50), blocks, connector);

        Assert.AreEqual(DropTarget.InsertAfter(0), result);
    }
}