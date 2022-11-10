using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    // TODO transfer prefas from gamemanager to here

    [Header("UI")]
    public GameObject DropdownOptionPrefab;
    public GameObject CheckboxOptionPrefab;
    public GameObject SliderOptionPrefab;
    public GameObject NumberInputOptionPrefab;
    public GameObject HeaderOptionPrefab;
    public GameObject SpaceOptionPrefab;

    private void OnEnable()
    {
        if(Instance == null) Instance = this;
    }
}
