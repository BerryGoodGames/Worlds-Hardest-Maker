using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using SFB;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveCurrentLevel()
    {
        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

        BinaryFormatter formatter = new();
        string path = StandaloneFileBrowser.SaveFilePanel("Save your level (.lvl)", Application.persistentDataPath,
            "MyLevel.lvl", "lvl");

        // check if user didn't pick any path
        if (path.Equals(""))
        {
            Debug.Log("Cancelled saving");
            return;
        }

        // create file
        FileStream stream = new(path, FileMode.Create);

        List<Data> levelData = SerializeCurrentLevel();

        formatter.Serialize(stream, levelData);
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
        // foreach (Transform anchor in ReferenceManager.Instance.AnchorContainer)
        // {
        //     AnchorDataOld anchorDataOld = new(anchor.GetComponentInChildren<PathControllerOld>(),
        //         anchor.GetComponentInChildren<AnchorControllerOld>().Container.transform);
        //     levelData.Add(anchorDataOld);
        // }
        foreach (Transform anchor in ReferenceManager.Instance.AnchorContainer)
        {
            AnchorData anchorData = new(anchor.GetComponent<AnchorParentController>().Child);
            levelData.Add(anchorData);
        }

        // serialize balls
        foreach (Transform ball in ReferenceManager.Instance.BallDefaultContainer)
        {
            BallData ballData = new(ball.GetChild(0).GetComponent<BallDefaultController>());
            levelData.Add(ballData);
        }

        // serialize ball circles
        foreach (Transform ball in ReferenceManager.Instance.BallCircleContainer)
        {
            BallCircleData ballCircleData = new(ball.GetChild(0).GetComponent<BallCircleController>());
            levelData.Add(ballCircleData);
        }

        // serialize coins
        foreach (Transform coin in ReferenceManager.Instance.CoinContainer)
        {
            CoinData coinData = new(coin.GetChild(0).GetComponent<CoinController>());
            levelData.Add(coinData);
        }

        // serialize keys
        foreach (Transform key in ReferenceManager.Instance.KeyContainer)
        {
            KeyData keyData = new(key.GetChild(0).GetComponent<KeyController>());
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
            Application.persistentDataPath, "lvl", false);

        // check if user selected nothing
        if (pathArr.Length != 1)
        {
            Debug.Log("Cancelled loading");
            return null;
        }

        string path = pathArr[0];

        // check if file exists
        if (!File.Exists(path))
        {
            Debug.LogError($"Save file not found in {path}");
            return null;
        }

        // // load / deserialize file
        BinaryFormatter formatter = new();
        FileStream stream = new(path, FileMode.Open);

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

        List<Data> data = formatter.Deserialize(stream) as List<Data>;
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