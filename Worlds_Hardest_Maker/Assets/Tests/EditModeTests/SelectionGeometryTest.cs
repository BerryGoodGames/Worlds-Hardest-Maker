using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class SelectionGeometryTest
{
    [Test]
    public void GetBounds_SinglePoint_ReturnsSamePoint()
    {
        Vector2 testingPoint = new(1, 4);
        
        (Vector2 lowest, Vector2 highest) = SelectionGeometry.GetBounds(testingPoint);
        
        Assert.AreEqual(testingPoint, highest);
        Assert.AreEqual(testingPoint, lowest);
    }
    
    [Test]
    public void GetBounds_UnorderedPoints_ReturnsMinAndMax()
    {
        Vector2[] points = 
        {
            new(5, 1),
            new(-2, 10),
            new(7, -3),
            new(0, 6)
        };

        (Vector2 lowest, Vector2 highest) = SelectionGeometry.GetBounds(points);

        Assert.AreEqual(new Vector2(-2, -3), lowest);
        Assert.AreEqual(new Vector2(7, 10), highest);
    }
    
    [Test]
    public void GetBoundsMatrix_RoundsCorrectly()
    {
        (Vector2Int lowest, Vector2Int highest) = SelectionGeometry.GetBoundsMatrix(
            new Vector2(1.2f, 2.8f),
            new Vector2(5.9f, 6.1f));

        Assert.AreEqual(new Vector2Int(2, 3), lowest);
        Assert.AreEqual(new Vector2Int(5, 6), highest);
    }
    
    [Test]
    public void GetFillRange_Matrix_SinglePoint_ReturnsOnePosition()
    {
        IReadOnlyList<Vector2> result = SelectionGeometry.GetFillArea(new(2, 3), new(2, 3), WorldPositionType.Matrix).Positions;

        CollectionAssert.AreEquivalent(new[]
        {
            new Vector2(2, 3)
        }, result);
    }

    [Test]
    public void GetFillRange_Grid_SinglePoint_ReturnsOnePosition()
    {
        IReadOnlyList<Vector2> result = SelectionGeometry.GetFillArea(new(2, 3), new(2, 3), WorldPositionType.Grid).Positions;

        CollectionAssert.AreEquivalent(new[]
        {
            new Vector2(2, 3)
        }, result);
    }

    [Test]
    public void GetFillRange_Grid_CorrectPositions()
    {
        IReadOnlyList<Vector2> result = SelectionGeometry.GetFillArea(new(-0.5f, 2), new(0.5f, 3.5f), WorldPositionType.Grid).Positions;
        
        CollectionAssert.AreEquivalent(new[]
        {
            new Vector2(-0.5f, 3.5f),
            new Vector2(0, 3.5f),
            new Vector2(0.5f, 3.5f),
            new Vector2(-0.5f, 3),
            new Vector2(0, 3),
            new Vector2(0.5f, 3),
            new Vector2(-0.5f, 2.5f),
            new Vector2(0, 2.5f),
            new Vector2(0.5f, 2.5f),
            new Vector2(-0.5f, 2),
            new Vector2(0, 2),
            new Vector2(0.5f, 2),
        }, result);
    }
    
    [Test]
    public void GetFillRange_Matrix_CorrectPositions()
    {
        IReadOnlyList<Vector2> result = SelectionGeometry.GetFillArea(new(-1, 2), new(1, 3), WorldPositionType.Matrix).Positions;
        
        CollectionAssert.AreEquivalent(new[]
        {
            new Vector2(-1, 3),
            new Vector2(0, 3),
            new Vector2(1, 3),
            new Vector2(-1, 2),
            new Vector2(0, 2),
            new Vector2(1, 2),
        }, result);
    }
    
    [Test]
    public void GetFillRange_TwoAdjacentMatrixCells_ReturnsFourPositions()
    {
        IReadOnlyList<Vector2> range = SelectionGeometry.GetFillArea(new(0, 0), new(1, 1), WorldPositionType.Matrix).Positions;
        Assert.AreEqual(4, range.Count);
    }

    [Test]
    public void GetBounds_EmptyList_Throws()
    {
        Assert.Throws<System.ArgumentException>(() => SelectionGeometry.GetBounds());
    }
}
