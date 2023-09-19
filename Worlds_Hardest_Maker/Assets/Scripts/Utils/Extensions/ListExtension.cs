using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
    public static LinkedListNode<T> NodeAt<T>(this LinkedList<T> list, int position)
    {
        LinkedListNode<T> mark = list.First;
        int i = 0;
        while (i < position)
        {
            mark = mark.Next;
            if (mark == null) return null;

            i++;
        }

        return mark;
    }

    public static void Print<T>(this List<T> list) =>
        Debug.Log($"[{string.Join(", ", list)}] --- Length: {list.Count}");
}