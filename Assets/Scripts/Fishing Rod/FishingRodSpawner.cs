using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodSpawner : MonoBehaviour
{
    [SerializeField] GameObject FishingRod;
    [SerializeField] GameObject currentRod;

    private void OnEnable()
    {
        //GameIdleState.onStateIdle += DespawnRod;
        //GameIdleState.onStateIdle += SpawnRod;
        
        GameLandingState.onStateLanding += DespawnRod;

    }
    void Start()
    {
        //SpawnRod();
    }

    public void SpawnRod()
    {
        Debug.Log("Spawning Rod");
        if (currentRod != null)
        {
            Destroy(currentRod);
        }
        currentRod = Instantiate(FishingRod, transform.position, Quaternion.identity);
        currentRod.name = "FishingRod";
        currentRod.GetComponent<FishingRod>().InitializeRod();
    }

    public void DespawnRod()
    {
        Debug.Log("Despawning Rod");
        if (currentRod != null)
        {
            Destroy(currentRod);
        }
    }
}
