using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// Phase 0 safety net from the refactor plan.
///
/// These are intentionally PlayMode smoke tests. They are not trying to be
/// beautiful unit tests; they are regression detectors for the current,
/// scene-driven anchor/block subsystem before the refactor starts.
/// </summary>
public sealed class AnchorBlockTests
{
    private const string CONFIG_RESOURCE_PATH = "AnchorBlockTestConfig";

    private AnchorBlockTestConfig config;
    private readonly List<AnchorController> createdAnchors = new();

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        config = Resources.Load<AnchorBlockTestConfig>(CONFIG_RESOURCE_PATH);
        Assert.IsNotNull(
            config,
            $"Missing Resources/{CONFIG_RESOURCE_PATH}.asset. Create it with " +
            "Assets > Create > Tests > Phase 0 Smoke Test Config."
        );

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(config.applicationSceneName),
            "AnchorBlockTestConfig.applicationSceneName is empty."
        );

        if (SceneManager.GetActiveScene().name != config.applicationSceneName)
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(config.applicationSceneName);
            Assert.IsNotNull(load, $"Could not start loading scene '{config.applicationSceneName}'.");
            yield return load;
        }

        // Give VContainer and all scene managers a frame to initialize.
        float deadline = Time.realtimeSinceStartup + 10f;
        while (AnchorManager.Instance == null && Time.realtimeSinceStartup < deadline)
            yield return null;

        Assert.IsNotNull(
            AnchorManager.Instance,
            "AnchorManager.Instance was not initialized. The configured scene must be the real game/editor bootstrap scene."
        );
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Never leave the application in play mode after a failed test.
        if (LevelSessionEditManager.Instance != null && LevelSessionEditManager.Instance.IsPlaying)
        {
            PlayManager.Instance.TogglePlay(false);
            LevelSessionEditManager.Instance.IsPlaying = false;
            LevelSessionEditManager.Instance.IsPlaytesting = false;
        }

        foreach (AnchorController anchor in createdAnchors.ToArray())
        {
            if (anchor == null) continue;
            if (AnchorManager.Instance != null)
            {
                AnchorManager.Instance.Remove(anchor);
            }
        }

        createdAnchors.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator Smoke_CreateAnchorAndAllExistingBlockKinds()
    {
        AnchorController anchor = CreateAnchor();
        yield return null;

        ReplaceBlocksWithAllExistingKinds(anchor);
        Assert.AreEqual(12, anchor.Blocks.Count, "The current project has 12 anchor block kinds in the supplied codebase.");

        // Select the anchor so the real editor UI path creates controllers from the
        // domain blocks. This catches missing prefab wiring / controller wiring too.
        AnchorManager.Instance.Select(anchor, false);
        yield return null;
        yield return null;

        Assert.IsNotNull(ReferenceManager.Instance, "ReferenceManager must exist in the configured game scene.");
        Assert.IsNotNull(ReferenceManager.Instance.MainChainController, "MainChainController is missing.");

        ReferenceManager.Instance.MainChainController.UpdateChildrenArray();
        Assert.AreEqual(
            12,
            ReferenceManager.Instance.MainChainController.Children.Count,
            "Every saved block should have produced one UI controller."
        );

        // The UI has now reconstructed the domain list from the Transform hierarchy.
        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();
        Assert.AreEqual(12, anchor.Blocks.Count);

        Type[] expectedTypes =
        {
            typeof(MoveBlock),
            typeof(TeleportBlock),
            typeof(LoopBlock),
            typeof(RotateBlock),
            typeof(StartRotatingBlock),
            typeof(StopRotatingBlock),
            typeof(MoveAndRotateBlock),
            typeof(WaitBlock),
            typeof(SetEaseBlock),
            typeof(SetSpeedBlock),
            typeof(SetRotationBlock),
            typeof(SetDirectionBlock),
        };

        CollectionAssert.AreEqual(
            expectedTypes,
            anchor.Blocks.Select(block => block.GetType()).ToArray(),
            "The UI round-trip changed the block kinds/order."
        );
    }

    [UnityTest]
    public IEnumerator Smoke_SaveAndLoadPreservesAllAnchorBlocks()
    {
        AnchorController anchor = CreateAnchor();
        yield return null;

        ReplaceBlocksWithAllExistingKinds(anchor);
        AnchorData beforeSave = new(anchor);

        string path = Path.Combine(
            SaveSystem.LevelSavePath,
            $"Phase0Smoke_{Guid.NewGuid():N}.lvl"
        );

        try
        {
            // Use the production serializer directly rather than opening a file dialog.
            LevelData data = new()
            {
                Info = new LevelInfo { Name = "Phase0SmokeTest", },
                Objects = new List<Data> { beforeSave, },
            };

            SaveSystem.SerializeLevelData(path, data);
            Assert.IsTrue(File.Exists(path), "Production level serializer did not create the save file.");

            LevelData loaded = SaveSystem.LoadLevel(path);
            Assert.IsNotNull(loaded, "Production level loader returned null.");

            AnchorData loadedAnchorData = loaded.Objects.OfType<AnchorData>().SingleOrDefault();
            Assert.IsNotNull(loadedAnchorData, "The saved level did not contain the test AnchorData.");

            AnchorManager.Instance.Remove(anchor);
            createdAnchors.Remove(anchor);
            yield return null;

            Vector2 reloadPosition = config.anchorSpawnPosition + new Vector2(2f, 0f);
            loadedAnchorData.ImportToLevel(reloadPosition);
            yield return null;

            AnchorController reloadedAnchor = FindAnchorNear(reloadPosition);
            Assert.IsNotNull(reloadedAnchor, "Loading AnchorData did not recreate the anchor in the scene.");
            createdAnchors.Add(reloadedAnchor);

            Assert.AreEqual(12, reloadedAnchor.Blocks.Count, "Save/load lost anchor blocks.");
            CollectionAssert.AreEqual(
                new[]
                {
                    typeof(MoveBlock),
                    typeof(TeleportBlock),
                    typeof(LoopBlock),
                    typeof(RotateBlock),
                    typeof(StartRotatingBlock),
                    typeof(StopRotatingBlock),
                    typeof(MoveAndRotateBlock),
                    typeof(WaitBlock),
                    typeof(SetEaseBlock),
                    typeof(SetSpeedBlock),
                    typeof(SetRotationBlock),
                    typeof(SetDirectionBlock),
                },
                reloadedAnchor.Blocks.Select(block => block.GetType()).ToArray(),
                "Save/load changed the block type sequence."
            );
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    [UnityTest]
    public IEnumerator Smoke_PlaytestExecutesAtLeastOneFullLoop()
    {
        AnchorController anchor = CreateAnchor();
        yield return null;

        // Small, deterministic execution chain:
        // SetSpeed -> Move -> Loop -> Move. Once the first Move finishes,
        // the anchor reaches the Loop and then executes the block after it again.
        anchor.Blocks = new LinkedList<AnchorBlock>();
        anchor.AppendBlock(new SetSpeedBlock(false, 1000f, SetSpeedBlock.Unit.UnitsPerSecond));
        anchor.AppendBlock(new LoopBlock(false));
        anchor.AppendBlock(new MoveBlock(anchor, false, new Vector2(0.1f, 0f)));
        anchor.AppendBlock(new MoveBlock(anchor, false, new Vector2(0.2f, 0f)));

        // Make the test exercise the application's actual edit->play transition.
        Assert.IsNotNull(GameManager.Instance, "GameManager must exist in the configured game scene.");
        PlayManager.Instance.TogglePlay(false);

        float deadline = Time.realtimeSinceStartup + config.loopTimeoutSeconds;
        while (anchor.LoopBlockNode == null && Time.realtimeSinceStartup < deadline)
            yield return null;

        Assert.IsNotNull(
            anchor.LoopBlockNode,
            "The test anchor never reached its LoopBlock during the playtest."
        );

        Assert.IsNotNull(anchor.CurrentExecutingBlock, "The anchor stopped executing after the loop.");
    }

    [UnityTest]
    public IEnumerator Smoke_DeleteBlockMidChainRemovesItFromTheDomainChain()
    {
        AnchorController anchor = CreateAnchor();
        yield return null;

        anchor.Blocks = new LinkedList<AnchorBlock>();
        anchor.AppendBlock(new MoveBlock(anchor, false, new Vector2(1f, 0f)));
        anchor.AppendBlock(new MoveBlock(anchor, false, new Vector2(2f, 0f)));
        anchor.AppendBlock(new MoveBlock(anchor, false, new Vector2(3f, 0f)));

        AnchorManager.Instance.Select(anchor, false);
        yield return null;
        yield return null;

        ReferenceManager.Instance.MainChainController.UpdateChildrenArray();
        Assert.AreEqual(3, ReferenceManager.Instance.MainChainController.Children.Count);

        AnchorBlockController toDelete = ReferenceManager.Instance.MainChainController.Children[1];
        toDelete.IsLocked = false;
        int blockCountBefore = anchor.Blocks.Count;

        toDelete.Delete();
        yield return null;

        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

        Assert.AreEqual(blockCountBefore - 1, anchor.Blocks.Count, "Deleting a block did not update AnchorController.Blocks.");
        Assert.AreEqual(2, ReferenceManager.Instance.MainChainController.Children.Count);
        Assert.IsFalse(
            ReferenceManager.Instance.MainChainController.Children.Contains(toDelete),
            "The deleted controller is still part of the chain hierarchy."
        );
    }

    [UnityTest]
    public IEnumerator Smoke_ReorderTwoBlocksChangesTheirChainOrder()
    {
        AnchorController anchor = CreateAnchor();
        yield return null;

        anchor.Blocks = new LinkedList<AnchorBlock>();
        anchor.AppendBlock(new MoveBlock(anchor, false, new Vector2(1f, 0f)));
        anchor.AppendBlock(new TeleportBlock(anchor, false, new Vector2(2f, 0f)));

        AnchorManager.Instance.Select(anchor, false);
        yield return null;
        yield return null;

        ReferenceManager.Instance.MainChainController.UpdateChildrenArray();
        Assert.AreEqual(2, ReferenceManager.Instance.MainChainController.Children.Count);

        AnchorBlockController first = ReferenceManager.Instance.MainChainController.Children[0];
        AnchorBlockController second = ReferenceManager.Instance.MainChainController.Children[1];

        // This is the same hierarchy mutation the drag-drop code ultimately performs:
        // the block is given a different sibling index, then the manager rebuilds the
        // ordered domain list from the hierarchy.
        int originalSecondSiblingIndex = second.transform.GetSiblingIndex();
        second.transform.SetSiblingIndex(Mathf.Max(1, originalSecondSiblingIndex - 1));

        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

        ReferenceManager.Instance.MainChainController.UpdateChildrenArray();

        Assert.AreSame(
            second,
            ReferenceManager.Instance.MainChainController.Children[0],
            "Reordering the UI hierarchy did not change the chain order."
        );
        Assert.AreNotSame(first, ReferenceManager.Instance.MainChainController.Children[0]);
        Assert.AreEqual(typeof(TeleportBlock), anchor.Blocks.First.Value.GetType());
        Assert.AreEqual(typeof(MoveBlock), anchor.Blocks.Last.Value.GetType());
    }

    [UnityTest]
    public IEnumerator Smoke_SaveLoadAndUiRoundTripStillAgreeOnBlockOrder()
    {
        AnchorController anchor = CreateAnchor();
        yield return null;

        ReplaceBlocksWithAllExistingKinds(anchor);
        AnchorManager.Instance.Select(anchor, false);
        yield return null;
        yield return null;

        ReferenceManager.Instance.MainChainController.UpdateChildrenArray();
        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

        Type[] uiTypes = anchor.Blocks.Select(block => block.GetType()).ToArray();

        string path = Path.Combine(
            SaveSystem.LevelSavePath,
            $"Phase0Smoke_{Guid.NewGuid():N}.lvl"
        );

        try
        {
            SaveSystem.SerializeLevelData(
                path,
                new LevelData
                {
                    Info = new LevelInfo { Name = "Phase0SmokeUiRoundTrip", },
                    Objects = new List<Data> { new AnchorData(anchor), },
                }
            );

            LevelData loaded = SaveSystem.LoadLevel(path);
            Assert.IsNotNull(loaded);

            AnchorData loadedAnchorData = loaded.Objects.OfType<AnchorData>().Single();

            AnchorManager.Instance.Remove(anchor);
            createdAnchors.Remove(anchor);
            yield return null;

            Vector2 reloadPosition = config.anchorSpawnPosition + new Vector2(3f, 0f);
            loadedAnchorData.ImportToLevel(reloadPosition);
            yield return null;

            AnchorController reloadedAnchor = FindAnchorNear(reloadPosition);
            Assert.IsNotNull(reloadedAnchor);
            createdAnchors.Add(reloadedAnchor);

            AnchorManager.Instance.Select(reloadedAnchor, false);
            yield return null;
            yield return null;

            AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

            CollectionAssert.AreEqual(
                uiTypes,
                reloadedAnchor.Blocks.Select(block => block.GetType()).ToArray(),
                "The persistence -> UI -> domain round-trip changed block order/type."
            );
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    private AnchorController CreateAnchor()
    {
        Assert.IsNotNull(AnchorManager.Instance, "AnchorManager is missing.");

        ManagerParameters args = new()
        {
            Position = config.anchorSpawnPosition,
        };

        AnchorController anchor = ((IManager<AnchorController>)AnchorManager.Instance).SetInSheet(args);
        Assert.IsNotNull(
            anchor,
            "Could not create the smoke-test anchor. Pick an empty anchorSpawnPosition in the config asset."
        );

        createdAnchors.Add(anchor);
        return anchor;
    }

    private static AnchorController FindAnchorNear(Vector2 position)
    {
        AnchorController[] anchors = UnityEngine.Object.FindObjectsByType<AnchorController>(FindObjectsSortMode.None);
        return anchors
            .OrderBy(anchor => ((Vector2)anchor.transform.position - position).sqrMagnitude)
            .FirstOrDefault(anchor => ((Vector2)anchor.transform.position - position).sqrMagnitude < 0.01f);
    }

    private static void ReplaceBlocksWithAllExistingKinds(AnchorController anchor)
    {
        anchor.Blocks = new LinkedList<AnchorBlock>();

        anchor.AppendBlock(new MoveBlock(anchor, false, new Vector2(1f, 0f)));
        anchor.AppendBlock(new TeleportBlock(anchor, false, new Vector2(2f, 0f)));
        anchor.AppendBlock(new LoopBlock(false));
        anchor.AppendBlock(new RotateBlock(false, 1f));
        anchor.AppendBlock(new StartRotatingBlock(false));
        anchor.AppendBlock(new StopRotatingBlock(false));
        anchor.AppendBlock(new MoveAndRotateBlock(anchor, false, new Vector2(3f, 0f), 1f, false));
        anchor.AppendBlock(new WaitBlock(false, 0.01f, WaitBlock.Unit.Seconds));
        anchor.AppendBlock(new SetEaseBlock(false, Ease.InBack));
        anchor.AppendBlock(new SetSpeedBlock(false, 5f, SetSpeedBlock.Unit.UnitsPerSecond));
        anchor.AppendBlock(new SetRotationBlock(false, 1f, RotationUnit.Iterations));
        anchor.AppendBlock(new SetDirectionBlock(false, true));
    }
}