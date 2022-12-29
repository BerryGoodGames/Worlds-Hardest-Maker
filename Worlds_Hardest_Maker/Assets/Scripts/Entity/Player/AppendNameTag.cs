using TMPro;
using UnityEngine;

/// <summary>
///     Script for player
///     to add a name tag following attached gameObject
///     and splitting gameObject and name tag
/// </summary>
public class AppendNameTag : MonoBehaviour
{
    [SerializeField] private bool showOnlyWhenMultiplayer = true;
    [SerializeField] private GameObject nameTagPrefab;

    [HideInInspector] public GameObject nameTag;

    private void Awake()
    {
        if (!GameManager.Instance.Multiplayer && showOnlyWhenMultiplayer) return;

        nameTag = Instantiate(nameTagPrefab, Vector2.zero, Quaternion.identity,
            ReferenceManager.Instance.nameTagContainer);

        UIFollowEntity followSettings = nameTag.GetComponent<UIFollowEntity>();
        followSettings.entity = gameObject;
        followSettings.offset = new(0, 0.6f);
    }

    public void SetNameTag(string name)
    {
        nameTag.GetComponent<TMP_Text>().text = name;
    }
}