using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] GameObject fishPrefab;
    [SerializeField] GameObject spawnLocation;

    public delegate void OnSpawn();
    public static OnSpawn onSpawn;

    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            Instantiate(fishPrefab, spawnLocation.transform.position, Quaternion.identity);
            onSpawn();
        }
    }
}
