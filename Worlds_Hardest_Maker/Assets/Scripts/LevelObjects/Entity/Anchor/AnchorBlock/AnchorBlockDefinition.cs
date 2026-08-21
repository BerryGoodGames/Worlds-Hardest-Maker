using UnityEngine;

[CreateAssetMenu(fileName = "NewAnchorBlock", menuName = "ScriptableObjects/Anchor Blocks/Definition")]
public class AnchorBlockDefinition : ScriptableObject
{
    public string TypeID;
    public GameObject Prefab;
}