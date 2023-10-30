using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockRebuilder : MonoBehaviour
{
    [SerializeField] private List<ContentSizeFitter> contentSizeFitters;

    public void RebuildLayout()
    {
        foreach (ContentSizeFitter contentSizeFitter in contentSizeFitters) { contentSizeFitter.Recalculate(); }
    }

    [ButtonMethod]
    public void CollectAllContentSizeFitters()
    {
        contentSizeFitters = new();
        TraverseChildren(transform as RectTransform);

        contentSizeFitters.Add(GetComponent<ContentSizeFitter>());
    }

    private void TraverseChildren(RectTransform parentTransform)
    {
        List<ContentSizeFitter> children = GetContentSizeFittersInChildren(parentTransform);

        if (children.Count == 0) return;

        foreach (ContentSizeFitter child in children)
        {
            // first add content size fitters of child's children
            TraverseChildren(child.transform as RectTransform);

            // then add content size fitter of child
            contentSizeFitters.Add(child);
        }
    }

    private static List<ContentSizeFitter> GetContentSizeFittersInChildren(RectTransform transform)
    {
        List<ContentSizeFitter> children = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.TryGetComponent(out ContentSizeFitter childContentSizeFitter)) children.Add(childContentSizeFitter);
        }

        return children;
    }
}