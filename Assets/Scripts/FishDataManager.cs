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
        BoidBehaviorData dataToSave = ConvertBoidBehaviotToData(fishStats);

        string json = JsonUtility.ToJson(dataToSave);
        string filePath = Application.persistentDataPath + "/fishData.json";
        File.AppendAllText(filePath, json + "\n");

        Debug.Log(json);
    }

    public BoidBehaviorData[] LoadFishData()
    {
        string filePath = Application.persistentDataPath + "/fishData.json";
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No fish data found.");
            return new BoidBehaviorData[0];
        }

        string[] jsonLines = File.ReadAllLines(filePath);

        //BoidBehaviorData[] fishStatsArray = new BoidBehaviorData[jsonLines.Length];

        //for (int i = 0; i < jsonLines.Length; i++)
        //{
            //fishStatsArray[i] = JsonUtility.FromJson<BoidBehaviorData>(jsonLines[i]);
        //}

        //return fishStatsArray;

        List<BoidBehaviorData> fishStatsList = new List<BoidBehaviorData>();

        foreach (string json in jsonLines)
        {
            BoidBehaviorData fishStats = JsonUtility.FromJson<BoidBehaviorData>(json);
            fishStatsList.Add(fishStats);
        }

        return fishStatsList.ToArray();
    }

    private BoidBehaviorData ConvertBoidBehaviotToData(BoidBehavior fish)
    {
        BoidBehaviorData data = new BoidBehaviorData();

        data.breed = fish.maidenName;
        data.size = fish.sizeMultiplier;
        data.level = fish.foodScore;
        data.combo = fish.comboMeter;
        //data.time = fish.timeCaught;
        data.sketch = fish.sketch; ;

        return data;
    }

}

[System.Serializable]
public class BoidBehaviorData
{
    public string breed;
    public float size;
    public float level;
    public float combo;
    public string time;
    public Sprite sketch;
}
