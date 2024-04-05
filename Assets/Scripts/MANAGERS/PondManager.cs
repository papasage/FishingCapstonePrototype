using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PondManager : MonoBehaviour
{
    private static PondManager _instance;
    public static PondManager Instance => _instance;

    private List<BoidBehavior> _allFish = new List<BoidBehavior>();

    private void Awake()
    {
        // Singleton pattern to ensure there's only one instance of FishManager
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogWarning("Duplicate FishManager instance found. Destroying the new one.");
            Destroy(gameObject);
        }
    }
    public void RegisterFish(BoidBehavior fish)
    {
        _allFish.Add(fish);
    }

    public void UnregisterFish(BoidBehavior fish)
    {
        _allFish.Remove(fish);
    }

    public List<BoidBehavior> GetAllFish()
    {
        return _allFish;
    }
}
