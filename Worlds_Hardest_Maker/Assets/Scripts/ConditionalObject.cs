using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class ConditionalObject : MonoBehaviour
{
    [SerializeField] private bool editOnly = true;

    private void Start()
    {
        if (TransitionManager.Instance == null) return;

        if (editOnly && !TransitionManager.Instance.IsEdit)
        {
            Destroy(gameObject);
        }
    }
}