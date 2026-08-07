using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using WorldsHardestMaker.PlayerRecording;
using WorldsHardestMaker.PlayerRecording.Recording;

public class PlayerRecordingAnalyzerTest
{
    private RecordingAnalyzer analyzer;
    private MockFrameStorage mockFrameStorage;

    [SetUp]
    public void Setup()
    {
        analyzer = new();
        mockFrameStorage = new();
    }

    [Test]
    public void AnalyzeFrames_SingleFrameNoEvents_MarksAsSuccessfulRun()
    {
        mockFrameStorage.AddFrame(new() { Position = Vector2.zero });

        analyzer.AnalyzeFrames(mockFrameStorage);

        Assert.AreEqual(1, analyzer.Frames.Count);
        Assert.IsTrue(analyzer.Frames[0].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[0].Died);
    }

    [Test]
    public void AnalyzeFrames_FramesAfterDeath_MarkAsStartOfSuccessfulRun()
    {
        mockFrameStorage.AddFrame(new() { Position = Vector2.zero, Died = false });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one, Died = true });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one * 2, Died = false });

        analyzer.AnalyzeFrames(mockFrameStorage);

        Assert.IsFalse(analyzer.Frames[0].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[1].StartsSuccessfulRun);
        Assert.IsTrue(analyzer.Frames[2].StartsSuccessfulRun);
    }

    [Test]
    public void AnalyzeFrames_CheckpointHit_MarkAsStartOfSuccessfulRun()
    {
        mockFrameStorage.AddFrame(new() { Position = Vector2.zero, Died = false });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one, CheckpointHit = true });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one * 2, Died = false });

        analyzer.AnalyzeFrames(mockFrameStorage);

        Assert.IsFalse(analyzer.Frames[0].StartsSuccessfulRun);
        Assert.IsTrue(analyzer.Frames[1].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[2].StartsSuccessfulRun);
    }

    [Test]
    public void AnalyzeFrames_MultipleDeath_RecoverySequences_CorrectlyIdentified()
    {
        // Segment 1: Death at frame 1, recovery at frame 2
        mockFrameStorage.AddFrame(new() { Position = Vector2.zero });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one, Died = true });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one * 2 });

        // Segment 2: Death at frame 4, recovery at frame 5
        mockFrameStorage.AddFrame(new() { Position = Vector2.one * 3, Died = true });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one * 4 });

        analyzer.AnalyzeFrames(mockFrameStorage);

        Assert.IsFalse(analyzer.Frames[0].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[1].StartsSuccessfulRun);
        Assert.IsTrue(analyzer.Frames[2].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[3].StartsSuccessfulRun);
        Assert.IsTrue(analyzer.Frames[4].StartsSuccessfulRun);
    }

    [Test]
    public void AnalyzeFrames_AllDeaths_NoCheckpoints_LastDeathDoesNotMarkSuccessfulRun()
    {
        mockFrameStorage.AddFrame(new() { Position = Vector2.zero });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one, Died = true });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one * 2, Died = true });

        analyzer.AnalyzeFrames(mockFrameStorage);

        Assert.IsFalse(analyzer.Frames[0].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[1].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[2].StartsSuccessfulRun);
    }

    [Test]
    public void AnalyzeFrames_DeathNotAtEnd_FollowedByFrames_MarksRecoveryAsSuccessful()
    {
        mockFrameStorage.AddFrame(new() { Position = Vector2.zero });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one, Died = true });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one * 2 });

        analyzer.AnalyzeFrames(mockFrameStorage);

        Assert.IsTrue(analyzer.Frames[2].StartsSuccessfulRun);
    }

    [Test]
    public void AnalyzeFrames_ContinuousSuccessfulSegmentBeforeDeath_MarksFirstAsSuccessful()
    {
        mockFrameStorage.AddFrame(new() { Position = Vector2.zero });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one * 2, Died = true });

        analyzer.AnalyzeFrames(mockFrameStorage);

        Assert.IsTrue(analyzer.Frames[0].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[1].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[2].StartsSuccessfulRun);
    }

    [Test]
    public void AnalyzeFrames_CheckpointAfterDeath_ResetsSuccessfulSegment()
    {
        mockFrameStorage.AddFrame(new() { Position = Vector2.zero });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one, Died = true });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one * 2, CheckpointHit = true });
        mockFrameStorage.AddFrame(new() { Position = Vector2.one * 3 });

        analyzer.AnalyzeFrames(mockFrameStorage);

        Assert.IsFalse(analyzer.Frames[0].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[1].StartsSuccessfulRun);
        Assert.IsTrue(analyzer.Frames[2].StartsSuccessfulRun);
        Assert.IsFalse(analyzer.Frames[3].StartsSuccessfulRun);
    }

    [Test]
    public void AnalyzeFrames_PreservesPositions_Deaths_Checkpoints()
    {
        Vector2 pos1 = new(5, 10);
        Vector2 pos2 = new(15, 20);

        mockFrameStorage.AddFrame(new() { Position = pos1, Died = true });
        mockFrameStorage.AddFrame(new() { Position = pos2, CheckpointHit = true });

        analyzer.AnalyzeFrames(mockFrameStorage);

        Assert.AreEqual(pos1, analyzer.Frames[0].Position);
        Assert.IsTrue(analyzer.Frames[0].Died);
        Assert.AreEqual(pos2, analyzer.Frames[1].Position);
        Assert.IsTrue(analyzer.Frames[1].CheckpointHit);
    }

    [Test]
    public void AnalyzeFrames_EmptyFrameStorage_ResultsInEmptyAnalyzedFrames()
    {
        analyzer.AnalyzeFrames(mockFrameStorage);

        Assert.AreEqual(0, analyzer.Frames.Count);
    }

    private class MockFrameStorage : IRecordingFrameStorage<RawFrame>
    {
        private readonly List<RawFrame> frames = new();

        public IReadOnlyList<RawFrame> Frames => frames.AsReadOnly();

        public void AddFrame(RawFrame frame)
        {
            frames.Add(frame);
        }
    }
}
