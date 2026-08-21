using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnchorBlockCatalog", menuName = "ScriptableObjects/Anchor Blocks/Catalog")]
public class AnchorBlockCatalog : ScriptableObject
{
    public List<AnchorBlockDefinition> Definitions;
}