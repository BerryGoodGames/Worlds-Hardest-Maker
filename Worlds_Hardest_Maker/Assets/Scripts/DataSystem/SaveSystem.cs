using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using SFB;
using UnityEngine;

public static class SaveSystem
{
    public static string LevelSavePath
    {
        get
        {
            // create path if it doesn't exist yet
            string path = Application.persistentDataPath + "/Levels/";

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return path;
        }
    }

    public static void SaveCurrentLevel() => SaveCurrentLevel(LevelSessionManager.Instance.LevelSessionPath);

    public static void SaveCurrentLevel(string path)
    {
        // check if user didn't pick any path
        if (path.Equals(""))
        {
            Debug.LogWarning("No Level Selected");
            return;
        }

        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

        // create file
        FileStream stream = new(path, FileMode.Create);

        try
        {
            LevelInfo levelInfo = LevelSessionManager.Instance.LoadedLevelData.Info;
            levelInfo.LastEdited = DateTime.Now;
            levelInfo.EditTime += LevelSessionManager.Instance.EditTime;
            levelInfo.PlayTime += LevelSessionManager.Instance.PlayTime;
            levelInfo.Deaths += LevelSessionManager.Instance.Deaths;
            levelInfo.Completions += LevelSessionManager.Instance.Completions;
            if (LevelSessionManager.Instance.BestCompletionTime < levelInfo.BestCompletionTime)
                levelInfo.BestCompletionTime = LevelSessionManager.Instance.BestCompletionTime;

            List<Data> levelObjects = SerializeCurrentLevel();

            LevelData levelData = new()
            {
                Info = levelInfo,
                Objects = levelObjects,
            };

            BinaryFormatter formatter = new();
            formatter.Serialize(stream, levelData);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);

            stream.Close();
            throw;
        }

        stream.Close();

        Debug.Log($"Saved level at {path}");
    }

    private static List<Data> SerializeCurrentLevel()
    {
        List<Data> levelData = new();

        // serialize players
        List<GameObject> players = PlayerManager.GetPlayers();
        foreach (GameObject player in players)
        {
            PlayerData playerData = new(player.GetComponent<PlayerController>());
            levelData.Add(playerData);
        }

        // serialize anchors
        foreach (Transform anchor in ReferenceManager.Instance.AnchorContainer)
        {
            AnchorData anchorData = new(anchor.GetComponent<AnchorParentController>().Child);
            levelData.Add(anchorData);
        }

        // serialize loose anchor balls
        foreach (AnchorBallController anchorBall in AnchorBallManager.Instance.AnchorBallListGlobal)
        {
            AnchorBallData anchorBallData = (AnchorBallData)anchorBall.GetData();
            levelData.Add(anchorBallData);
        }

        // serialize coins
        foreach (CoinController coin in CoinManager.Instance.Coins)
        {
            CoinData coinData = new(coin);
            levelData.Add(coinData);
        }

        // serialize keys
        foreach (KeyController key in KeyManager.Instance.Keys)
        {
            KeyData keyData = new(key);
            levelData.Add(keyData);
        }

        // serialize fields
        foreach (Transform field in ReferenceManager.Instance.FieldContainer)
        {
            FieldData fieldData = new(field.gameObject);
            levelData.Add(fieldData);
        }

        // serialize current level settings
        levelData.Add(new LevelSettingsData(LevelSettings.Instance));

        return levelData;
    }

    public static LevelData LoadLevel()
    {
        // requests path from user and returns level in form of List<IData>
        string[] pathArr = StandaloneFileBrowser.OpenFilePanel(
            "Select your level (.lvl)",
            LevelSavePath, "lvl", false
        );

        // check if user selected nothing
        if (pathArr.Length != 1)
        {
            Debug.Log("Cancelled loading");
            return null;
        }

        string path = pathArr[0];

        return LoadLevel(path);
    }

    public static LevelData LoadLevel(string path)
    {
        // check if file exists
        if (!File.Exists(path))
        {
            Debug.LogError($"Save file not found in path \"{path}\"");
            return null;
        }

        // // load / deserialize file
        BinaryFormatter formatter = new();
        FileStream stream = new(path, FileMode.Open);
        LevelData data;

        try { data = formatter.Deserialize(stream) as LevelData; }
        catch
        {
            Debug.LogWarning($"Failed to load file at path: {path}");
            stream.Close();
            throw;
        }

        stream.Close();

        return data;
    }

    public static void SendLevel(string path)
    {
        string content = File.ReadAllText(path);

        // RPC file content to other clients while loading oneself
        GameManager.Instance.photonView.RPC("ReceiveLevel", RpcTarget.Others, content);
    }
}

[Serializable]
public abstract class Data
{
    public abstract void ImportToLevel();

    public virtual void ImportToLevel(Vector2 pos) =>
        Debug.LogWarning("ImportToLevel(Vector2 pos) has been called, but there is no override defined");

    public abstract EditMode GetEditMode();
}