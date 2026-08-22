using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnchorBlockCatalog", menuName = "ScriptableObjects/Anchor Blocks/Catalog")]
public class AnchorBlockCatalog : ScriptableObject
{
    [SerializeField] private List<AnchorBlockDefinition> definitions = new();
    
    private readonly Dictionary<string, AnchorBlockDefinition> idToDefinitionMap = new();

    public GameObject GetPrefabByID(string typeID)
    {
        if (!idToDefinitionMap.TryGetValue(typeID, out AnchorBlockDefinition definition))
        {
            throw new ArgumentOutOfRangeException(
                $"Could not get prefab for anchor block, given type ID ({typeID}) does not exist");
        }
        return definition.Prefab;
    }

    private void OnEnable()
    {
        foreach (AnchorBlockDefinition d in definitions)
        {
            if (!idToDefinitionMap.TryAdd(d.TypeID, d))
            {
                Debug.LogWarning($"Could not map anchor block type id {d.TypeID} to its definition. (Is there a duplicate??)");
            }
        }
    }
}