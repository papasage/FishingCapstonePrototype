using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishDataHandler : MonoBehaviour, IDataPersistence
{
    public BoidBehavior fishToSave;
    public BoidBehavior fishToLoad;

    public static FishDataHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FishDataHandler in the scene!");
        }
        Instance = this;
    }


    // I would have put this inside of the GameStateMachine.cs because that is where the fish are triggered to be saved,
    // but this method required the saving script to have MonoBehavior. So this gets its own script.
    // It is another singleton-style manager script that other scripts can call to like this:
    // FishDataHandler.instance.SaveFish(caughtFish);
    // that command will send the caughtFish through here, and it will be passed when it is called by DataPersistenceManager.cs


    //THIS METHOD IS CALLED BY OUTSIDE SOURCES THAT WANT TO SAVE AN INDIVIDUAL FISH
    public void SaveFish(BoidBehavior fishtosave)
    {
        fishtosave = fishToSave;
    }

    //THIS METHOD IS CALLED BY OUTSIDE SOURCES THAT WANT TO LOAD AN INDIVIDUAL FISH
    public BoidBehavior LoadFish()
    {
        return fishToLoad;
    }

    // Every script that uses IDataPersistence will have these methods run when DataPersistenceManager.cs says to SaveGame() or LoadGame()
    public void SaveData(ref GameData data)
    {
        data.savedFish = this.fishToSave;
    }
    public void LoadData(GameData data)
    {
        this.fishToLoad = data.savedFish;
    }


}
