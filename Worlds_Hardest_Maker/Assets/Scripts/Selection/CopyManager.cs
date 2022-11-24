using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class CopyManager : MonoBehaviour
{
    private static List<CopyData> copyDataList = new();

    public static Vector2 size = Vector2.zero;

    public static void Copy(Vector2 lowestPos, Vector2 highestPos)
    {
        Vector2 castPos = Vector2.Lerp(lowestPos, highestPos, 0.5f);
        Vector2 castSize = highestPos - lowestPos;

        size = castSize;

        Collider2D[] hits = Physics2D.OverlapBoxAll(castPos, castSize, 0, 3200);

        copyDataList.Clear();

        foreach(Collider2D hit in hits)
        {
            if(hit.TryGetComponent(out Controller controller))
            {
                IData data = controller.GetData();

                CopyData copyData = new(data, (Vector2)hit.transform.position - lowestPos);
                copyDataList.Add(copyData);
            }
        }
    }

    public static void Paste(Vector2 pos)
    {
        foreach(CopyData copyData in copyDataList)
        {
            copyData.Paste(pos);
        }
    }

}
