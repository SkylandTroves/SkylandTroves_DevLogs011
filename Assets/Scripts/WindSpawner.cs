
using UnityEngine;
using System.Collections.Generic;

public class WindSpawner : MonoBehaviour
{
    [Header("Wind Prefabs")]
    [SerializeField] private GameObject[] windPrefabs;
    
    [Header("Spawn Settings")]
    [SerializeField] private int maxWindEffects = 5;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float minLifetime = 5f;
    [SerializeField] private float maxLifetime = 10f;
    
    [Header("Position Settings")]
    [SerializeField] private float xRange = 10f;  
    [SerializeField] private float zRange = 10f; 
    [SerializeField] private float minY = 0f;
    [SerializeField] private float maxY = 5f;
    
    private List<GameObject> activeWindEffects = new List<GameObject>();
    private float nextSpawnTime;
    
    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }
    
    void Update()
    {
        for (int i = activeWindEffects.Count - 1; i >= 0; i--)
        {
            if (activeWindEffects[i] == null)
            {
                activeWindEffects.RemoveAt(i);
            }
        }
        
        if (Time.time >= nextSpawnTime && activeWindEffects.Count < maxWindEffects && windPrefabs.Length > 0)
        {
            SpawnWindEffect();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    void SpawnWindEffect()
    {
        int prefabIndex = Random.Range(0, windPrefabs.Length);
        GameObject prefab = windPrefabs[prefabIndex];
        
        if (prefab == null)
            return;
        
        Vector3 spawnPosition = CalculateSpawnPosition();
        
        GameObject windEffect = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        activeWindEffects.Add(windEffect);
        
        float lifetime = Random.Range(minLifetime, maxLifetime);
        Destroy(windEffect, lifetime);
    }
    
    Vector3 CalculateSpawnPosition()
    {
        Vector3 basePosition = transform.position;
        float xOffset = Random.Range(-xRange, xRange);
        float zOffset = Random.Range(-zRange, zRange);
        float y = Random.Range(minY, maxY);
        
        return new Vector3(
            basePosition.x + xOffset,
            basePosition.z + zOffset
        );
    }
    
   
}