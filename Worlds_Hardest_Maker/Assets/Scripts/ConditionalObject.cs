using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class ConditionalObject : MonoBehaviour
{
    [SerializeField] private bool editOnly = true;

    private void Start()
    {
        if (editOnly && !LevelSessionManager.Instance.IsEdit)
        {
            Destroy(gameObject);
        }
    }
}