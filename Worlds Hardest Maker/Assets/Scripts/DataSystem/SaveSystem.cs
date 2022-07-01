using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SFB;

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

            // serialize player
            GameObject player = PlayerManager.GetCurrentPlayer();
            if (player != null)
            {
                PlayerData playerData = new(player.GetComponent<PlayerController>());
                levelData.Add(playerData);
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

            formatter.Serialize(stream, levelData);
            stream.Close();

            Debug.Log($"Saved level at {path}");
        }
        else
        {
            Debug.Log("Cancelled saving");
        }
    }

    public static List<IData> LoadLevel()
    {
        // string path = Application.persistentDataPath + "/level.lvl";
        string[] pathArr = StandaloneFileBrowser.OpenFilePanel("Select your level (.lvl)", Application.persistentDataPath, "lvl", false);
        if (pathArr.Length > 0)
        {
            string path = pathArr[0];

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

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
    }
}

[System.Serializable]
public abstract class IData
{
    public abstract void CreateObject();
}