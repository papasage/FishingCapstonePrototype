using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FishDataManager : MonoBehaviour
{
    public static FishDataManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SaveFishData(BoidBehavior fishStats)
    {
        string json = JsonUtility.ToJson(fishStats);
        string filePath = Application.persistentDataPath + "/fishData.json";
        File.AppendAllText(filePath, json + "\n");

        Debug.Log(json);
    }

    public BoidBehavior[] LoadFishData()
    {
        string filePath = Application.persistentDataPath + "/fishData.json";
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No fish data found.");
            return new BoidBehavior[0];
        }

        string[] jsonLines = File.ReadAllLines(filePath);
        BoidBehavior[] fishStatsArray = new BoidBehavior[jsonLines.Length];

        for (int i = 0; i < jsonLines.Length; i++)
        {
            fishStatsArray[i] = JsonUtility.FromJson<BoidBehavior>(jsonLines[i]);
        }

        return fishStatsArray;
    }

}
