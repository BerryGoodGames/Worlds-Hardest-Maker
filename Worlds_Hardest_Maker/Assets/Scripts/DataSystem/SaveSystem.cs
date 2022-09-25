using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SFB;
using Photon.Pun;
using System.Runtime.InteropServices;
//#if UNITY_WEBGL && !UNITY_EDITOR
//using System.Text;
//using System.Runtime.InteropServices;
//#endif

public static class SaveSystem
{
    public static void SaveCurrentLevel()
    {
        BinaryFormatter formatter = new();
        string path = StandaloneFileBrowser.SaveFilePanel("Save your level (.lvl)", Application.persistentDataPath, "MyLevel.lvl", "lvl");
        if(!path.Equals(""))
        {
            FileStream stream = new(path, FileMode.Create);

            List<IData> levelData = new();

            // serialize players
            List<GameObject> players = PlayerManager.GetPlayers();
            foreach(GameObject player in players)
            {
                PlayerData playerData = new(player.GetComponent<PlayerController>());
                levelData.Add(playerData);
            }

            // serialize anchors
            foreach (Transform anchor in GameManager.Instance.AnchorContainer.transform)
            {
                AnchorData anchorData = new(anchor.GetComponentInChildren<PathController>(), anchor.GetComponentInChildren<AnchorController>().container.transform);
                levelData.Add(anchorData);
            }

            // serialize balls
            foreach (Transform ball in GameManager.Instance.BallDefaultContainer.transform)
            {
                BallData ballData = new(ball.GetChild(0).GetComponent<BallController>());
                levelData.Add(ballData);
            }

            // serialize ball circles
            foreach (Transform ball in GameManager.Instance.BallCircleContainer.transform)
            {
                BallCircleData ballCircleData = new(ball.GetChild(0).GetComponent<BallCircleController>());
                levelData.Add(ballCircleData);
            }

            // serialize coins
            foreach (Transform coin in GameManager.Instance.CoinContainer.transform)
            {
                CoinData coinData = new(coin.GetChild(0).GetComponent<CoinController>());
                levelData.Add(coinData);
            }
            // serialize keys
            foreach (Transform key in GameManager.Instance.KeyContainer.transform)
            {
                KeyData keyData = new(key.GetChild(0).GetComponent<KeyController>());
                levelData.Add(keyData);
            }

            // serialize fields
            foreach (Transform field in GameManager.Instance.FieldContainer.transform)
            {
                if (field.transform.CompareTag("OneWayField"))
                {
                    OneWayData fieldData = new(field.gameObject);
                    levelData.Add(fieldData);
                }
                else
                {
                    FieldData fieldData = new(field.gameObject);
                    levelData.Add(fieldData);
                }
            }

            // serialize current level settings
            levelData.Add(new LevelSettingsData(LevelSettings.Instance));
            Debug.Log(LevelSettings.Instance.drownDuration);

            formatter.Serialize(stream, levelData);
            stream.Close();

            Debug.Log($"Saved level at {path}");
        }
        else
        {
            Debug.Log("Cancelled saving");
        }
    }

//#if UNITY_WEBGL
//    [DllImport("__Internal")]
//    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

//    public static void OnFileUpload(string url) {
//        Debug.Log(url);
//    }
//#endif

    public static List<IData> LoadLevel()
    {
        //#if UNITY_WEBGL
        //                Debug.Log("opening in webgl");
        //                UploadFile("SaveSystem", "OnFileUpload", ".lvl", false);
        //                return null;
        //#else
        //string path = Application.persistentDataPath + "/level.lvl";

        string[] pathArr = StandaloneFileBrowser.OpenFilePanel("Select your level (.lvl)", Application.persistentDataPath, "lvl", false);

        if (pathArr.Length > 0)
        {
            string path = pathArr[0];

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new();
                //FileStream stream = new(path, FileMode.Open);
                //using FileStream stream = File.OpenWrite(path);
                FileStream stream = new(path, FileMode.Open);

                if (GameManager.Instance.Multiplayer)
                {
                    // RPC to every other client with path
                    SendLevel(path);
                }

                List<IData> data = formatter.Deserialize(stream) as List<IData>;
                stream.Close();

                return data;
            }
            else
            {
                Debug.LogError($"Save file not found in {path}");
                return null;
            }
        }
        else
        {
            Debug.Log("Cancelled loading");
            return null;
        }
//#endif
    }

    public static void SendLevel(string path)
    {
        string content = File.ReadAllText(path);

        // RPC file content to other clients while loading oneself
        GameManager.Instance.photonView.RPC("ReceiveLevel", RpcTarget.Others, content);
    }
}

[System.Serializable]
public abstract class IData
{
    public abstract void ImportToLevel();
}