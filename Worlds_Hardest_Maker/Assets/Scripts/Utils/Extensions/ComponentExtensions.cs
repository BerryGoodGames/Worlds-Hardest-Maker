using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static T[] GetComponentsInDirectChildren<T>(this Component parent) where T : Component
    {
        List<T> components = new();

        foreach (Transform child in parent.transform)
        {
            components.Add(child.GetComponent<T>());
        }

        return components.ToArray();
    }
}