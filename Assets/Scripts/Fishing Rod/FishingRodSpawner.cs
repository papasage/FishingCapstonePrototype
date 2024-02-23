using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodSpawner : MonoBehaviour
{
    [SerializeField] GameObject FishingRod;
    [SerializeField] GameObject currentRod;
    UIController ui;

    private void OnEnable()
    {
        //GameIdleState.onStateIdle += DespawnRod;
        //GameIdleState.onStateIdle += SpawnRod;
        
        GameLandingState.onStateLanding += DespawnRod;

    }
    void Start()
    {
        //SpawnRod();
        ui = GameObject.Find("UIManager").GetComponent<UIController>();
        ui.rodIsEquipped = false;
    }

    public void SpawnRod()
    {
        Debug.Log("Spawning Rod");
        if (currentRod != null)
        {
            DespawnRod();
        }
        currentRod = Instantiate(FishingRod, transform.position, Quaternion.identity);
        currentRod.name = "FishingRod";
        currentRod.GetComponent<FishingRod>().InitializeRod();
        
        //tell the UI that it is okay to look for references to the rod
        ui.rodIsEquipped = true;
        ui.InitializeRodUI(currentRod.GetComponent<FishingRod>().rodToBobberStringSlack);
    }

    public void DespawnRod()
    {
        ui.rodIsEquipped = false;

        if (currentRod != null)
        {
            Debug.Log("Despawning Rod");
            
            Destroy(currentRod);
            
        }
    }
}
