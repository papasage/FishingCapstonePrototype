using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] GameObject fishPrefab;
    [SerializeField] GameObject spawnLocation;

    public delegate void OnSpawn();
    public static OnSpawn onSpawn;
    private List<GameObject> boids;
    private int BoidCount;
    public int pondBoidMax = 64;
    public int spawnFrequencyInSeconds = 20;


    private void Start()
    {
        StartCoroutine(FishSpawnCountdown());
    }
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            SpawnFish();
        }
    }

    void CheckBoidCount()
    {
        List<BoidBehavior> boids = PondManager.Instance.GetAllFish();

        BoidCount = 0;

        foreach (BoidBehavior boid in boids)
        {

            if (boid != null)
            {
                if (!boid.isDead)
                {
                    BoidCount++;
                }
            }
        }
    }

    void SpawnFish()
    {
        Instantiate(fishPrefab, spawnLocation.transform.position, Quaternion.identity, GameObject.Find("[FISH SPAWNER]").transform);
        onSpawn();
    }

    IEnumerator FishSpawnCountdown()
    {
        yield return new WaitForSeconds(spawnFrequencyInSeconds);

        CheckBoidCount();
        

        if (BoidCount < pondBoidMax)
        {
            //Debug.Log("BOID RESPAWN");
            SpawnFish();
        }

        StartCoroutine(FishSpawnCountdown());

    }
}
