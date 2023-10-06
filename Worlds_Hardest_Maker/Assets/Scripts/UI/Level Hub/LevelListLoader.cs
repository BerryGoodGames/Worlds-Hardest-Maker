using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelListLoader : MonoBehaviour
{
    [SerializeField] private bool refresh;
    [SerializeField][ConditionalField(nameof(refresh))] private float refreshInterval = 5;

    [Separator("References")] [SerializeField]
    private GameObject levelCardPrefab;

    private FileInfo[] prevLevels;

    private void Awake()
    {
        StartCoroutine(LoadCoroutine());
    }

    private IEnumerator LoadCoroutine()
    {
        while (true)
        {
            if(refresh) Refresh();

            // wait until trying to load levels again
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    [ButtonMethod]
    private void Refresh()
    {
        DirectoryInfo levelDirectory = new(SaveSystem.LevelSavePath);

        FileInfo[] levelInfo = levelDirectory.GetFiles("*.lvl");

        if (prevLevels == null || !levelInfo.SequenceEqual(prevLevels))
        {
            UpdateLevelCards(levelInfo);
        }

        prevLevels = levelInfo;
    }

    private void UpdateLevelCards(IEnumerable<FileInfo> levelInfo)
    {
        // destroy all level cards
        foreach (Transform t in transform)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(t.gameObject);
                continue;
            }
#endif

            Destroy(t.gameObject);
        }

        foreach (FileInfo info in levelInfo)
        {
            // create new level cards
            LevelCardController levelCard = Instantiate(levelCardPrefab, transform).GetComponent<LevelCardController>();

            // level card settings
            levelCard.Name = info.Name[..^4]; // remove .lvl at end
            levelCard.LevelPath = info.FullName;
        }
    }
}
