using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for player
/// to add a name tag following attached gameobject
/// and splitting gameobject and name tag
/// </summary>
public class AppendNameTag : MonoBehaviour
{
    [SerializeField] private bool showOnlyWhenMultiplayer = true;
    [SerializeField] private GameObject nameTagPrefab;
    [HideInInspector] public GameObject nameTag;

    private void Awake()
    {
        if (!GameManager.Instance.Multiplayer && showOnlyWhenMultiplayer) return;

        nameTag = Instantiate(nameTagPrefab, Vector2.zero, Quaternion.identity, GameManager.Instance.NameTagContainer.transform);

        UIFollowEntity followSettings = nameTag.GetComponent<UIFollowEntity>();
        followSettings.entity = gameObject;
        followSettings.offset = new(0, 0.6f);
    }

    public void SetNameTag(string name)
    {
        nameTag.GetComponent<TMPro.TMP_Text>().text = name;
    }
}
