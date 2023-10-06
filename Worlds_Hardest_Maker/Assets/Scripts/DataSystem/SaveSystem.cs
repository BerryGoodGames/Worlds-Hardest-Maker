using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
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
            string path = Application.persistentDataPath + "/Levels";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }

    public static void SaveCurrentLevel()
    {
        SaveCurrentLevel(LevelHubManager.LoadedLevelPath);
    }

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
            List<Data> levelData = SerializeCurrentLevel();
            BinaryFormatter formatter = new();
            formatter.Serialize(stream, levelData);
        }
        catch
        {
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
        foreach (Transform ball in ReferenceManager.Instance.AnchorBallContainer.transform)
        {
            AnchorBallData anchorBallData = new(ball.GetChild(0).localPosition);
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

    public static List<Data> LoadLevel(bool updateDiscordActivity = true)
    {
        // requests path from user and returns level in form of List<IData>
        string[] pathArr = StandaloneFileBrowser.OpenFilePanel("Select your level (.lvl)",
            LevelSavePath, "lvl", false);

        // check if user selected nothing
        if (pathArr.Length != 1)
        {
            Debug.Log("Cancelled loading");
            return null;
        }

        string path = pathArr[0];

        return LoadLevel(path, updateDiscordActivity);
    }

    public static List<Data> LoadLevel(string path, bool updateDiscordActivity = true)
    {
        // check if file exists
        if (!File.Exists(path))
        {
            Debug.LogError($"Save file not found in {path}");
            return null;
        }

        // // load / deserialize file
        BinaryFormatter formatter = new();
        FileStream stream = new(path, FileMode.Open);
        List<Data> data;

        try
        {
            if (MultiplayerManager.Instance.Multiplayer)
                // RPC to every other client with path
                SendLevel(path);

            // set discord activity
            if (updateDiscordActivity)
            {
                string[] splitPath = stream.Name.Split("\\");
                string levelName = splitPath[^1].Replace(".lvl", "");
                // DiscordManager.State = $"Last opened Level: {levelName}";
            }

            //using (StreamReader reader = new StreamReader(stream))
            //{
            //    Debug.Log(reader.ReadToEnd());
            //}

            data = formatter.Deserialize(stream) as List<Data>;
        }
        catch
        {
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