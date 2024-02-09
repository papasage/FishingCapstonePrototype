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
        boids = new List<GameObject>(GameObject.FindObjectsOfType<BoidBehavior>().Select(boid => boid.gameObject));

        BoidCount = 0;
        foreach (GameObject boid in boids)
        {
            BoidBehavior boidBehavior = boid.GetComponent<BoidBehavior>();

            if (boidBehavior != null)
            {
                if (boidBehavior.isDead == false)
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
        yield return new WaitForSeconds(30);

        CheckBoidCount();
        

        if (BoidCount < 60)
        {
            Debug.Log("BOID RESPAWN");
            SpawnFish();
        }
        else Debug.Log("MAX BOIDS. RESPAWN TIMER BLOCKED");

        StartCoroutine(FishSpawnCountdown());

    }
}
