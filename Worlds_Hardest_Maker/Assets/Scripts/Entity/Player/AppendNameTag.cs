using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     Script for player
///     to add a name tag following attached gameObject
///     and splitting gameObject and name tag
/// </summary>
public class AppendNameTag : MonoBehaviour
{
    [SerializeField] private bool showOnlyWhenMultiplayer = true;
    [SerializeField] private GameObject nameTagPrefab;

    [FormerlySerializedAs("nameTag")] [HideInInspector]
    public GameObject NameTag;

    private void Awake()
    {
        if (!MultiplayerManager.Instance.Multiplayer && showOnlyWhenMultiplayer) return;

        NameTag = Instantiate(nameTagPrefab, Vector2.zero, Quaternion.identity,
            ReferenceManager.Instance.NameTagContainer);

        UIFollowEntity followSettings = NameTag.GetComponent<UIFollowEntity>();
        followSettings.Entity = gameObject;
        followSettings.Offset = new(0, 0.6f);
    }

    public void SetNameTag(string name)
    {
        NameTag.GetComponent<TMP_Text>().text = name;
    }
}