using System;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class AnchorBlockCatalogTest
{
    private AnchorBlockCatalog catalog;
    private GameObject prefabA;
    private GameObject prefabB;

    [SetUp]
    public void SetUp()
    {
        catalog = ScriptableObject.CreateInstance<AnchorBlockCatalog>();

        prefabA = new GameObject("Prefab_A");
        prefabB = new GameObject("Prefab_B");
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(catalog);
        UnityEngine.Object.DestroyImmediate(prefabA);
        UnityEngine.Object.DestroyImmediate(prefabB);
    }

    [Test]
    public void GetPrefabByID_ReturnsCorrectPrefab()
    {
        AnchorBlockDefinition definition = CreateDefinition("anchor_a", prefabA);
        SetDefinitions(definition);

        Assert.That(catalog.GetPrefabByID("anchor_a"), Is.SameAs(prefabA));
    }

    [Test]
    public void GetPrefabByID_UnknownID_ThrowsArgumentOutOfRangeException()
    {
        AnchorBlockDefinition definition = CreateDefinition("anchor_a", prefabA);
        SetDefinitions(definition);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => catalog.GetPrefabByID("does_not_exist"));
    }

    [Test]
    public void GetPrefabByID_WithMultipleDefinitions_ReturnsMatchingPrefab()
    {
        AnchorBlockDefinition definitionA = CreateDefinition("anchor_a", prefabA);
        AnchorBlockDefinition definitionB = CreateDefinition("anchor_b", prefabB);

        SetDefinitions(definitionA, definitionB);

        Assert.That(catalog.GetPrefabByID("anchor_a"), Is.SameAs(prefabA));
        Assert.That(catalog.GetPrefabByID("anchor_b"), Is.SameAs(prefabB));
    }

    [Test]
    public void OnEnable_WithDuplicateIDs_UsesFirstDefinition()
    {
        AnchorBlockDefinition first = CreateDefinition("duplicate", prefabA);
        AnchorBlockDefinition second = CreateDefinition("duplicate", prefabB);

        LogAssert.Expect(
            LogType.Warning,
            "Could not map anchor block type id duplicate to its definition. (Is there a duplicate??)");

        SetDefinitions(first, second);

        Assert.That(catalog.GetPrefabByID("duplicate"), Is.SameAs(prefabA));
    }

    private AnchorBlockDefinition CreateDefinition(string typeID, GameObject prefab)
    {
        AnchorBlockDefinition definition =
            ScriptableObject.CreateInstance<AnchorBlockDefinition>();

        definition.TypeID = typeID;
        definition.Prefab = prefab;

        return definition;
    }

    private void SetDefinitions(params AnchorBlockDefinition[] definitions)
    {
        // `definitions` is private, so use Unity's serialization system
        // rather than relying on implementation-specific reflection to
        // modify the field.
        SerializedObject serializedCatalog = new SerializedObject(catalog);
        SerializedProperty definitionsProperty =
            serializedCatalog.FindProperty("definitions");

        definitionsProperty.arraySize = definitions.Length;

        for (int i = 0; i < definitions.Length; i++)
        {
            definitionsProperty.GetArrayElementAtIndex(i).objectReferenceValue =
                definitions[i];
        }

        serializedCatalog.ApplyModifiedPropertiesWithoutUndo();

        // CreateInstance() already called OnEnable before we populated
        // the serialized definitions, so invoke it again.
        InvokeOnEnable();
    }

    private void InvokeOnEnable()
    {
        MethodInfo onEnable = typeof(AnchorBlockCatalog).GetMethod(
            "OnEnable",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(onEnable, Is.Not.Null);

        onEnable.Invoke(catalog, null);
    }
}