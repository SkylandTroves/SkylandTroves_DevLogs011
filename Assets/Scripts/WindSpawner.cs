
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
    [SerializeField] private float xRange = 10f;  // Will spawn at transform.position.x ± this value
    [SerializeField] private float zRange = 10f;  // Will spawn at transform.position.z ± this value
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
        // Clean up destroyed effects from the list
        for (int i = activeWindEffects.Count - 1; i >= 0; i--)
        {
            if (activeWindEffects[i] == null)
            {
                activeWindEffects.RemoveAt(i);
            }
        }
        
        // Check if it's time to spawn a new effect
        if (Time.time >= nextSpawnTime && activeWindEffects.Count < maxWindEffects && windPrefabs.Length > 0)
        {
            SpawnWindEffect();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    void SpawnWindEffect()
    {
        // Select random wind prefab
        int prefabIndex = Random.Range(0, windPrefabs.Length);
        GameObject prefab = windPrefabs[prefabIndex];
        
        if (prefab == null)
            return;
        
        // Calculate spawn position relative to this transform
        Vector3 spawnPosition = CalculateSpawnPosition();
        
        // Instantiate wind effect
        GameObject windEffect = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        activeWindEffects.Add(windEffect);
        
        // Set random lifetime
        float lifetime = Random.Range(minLifetime, maxLifetime);
        Destroy(windEffect, lifetime);
    }
    
    Vector3 CalculateSpawnPosition()
    {
        // Get current position of the Wind game object
        Vector3 basePosition = transform.position;
        
        // Generate random offsets within the specified ranges
        float xOffset = Random.Range(-xRange, xRange);
        float zOffset = Random.Range(-zRange, zRange);
        float y = Random.Range(minY, maxY);
        
        // Calculate final position
        return new Vector3(
            basePosition.x + xOffset,
            y,  // Use absolute Y, not relative to base position
            basePosition.z + zOffset
        );
    }
    
    // Editor helper method to visualize spawn area
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        
        Vector3 basePosition = transform.position;
        float minX = basePosition.x - xRange;
        float maxX = basePosition.x + xRange;
        float minZ = basePosition.z - zRange;
        float maxZ = basePosition.z + zRange;
        
        // Draw bottom rectangle
        Vector3 bottomCenter = new Vector3((minX + maxX) / 2, minY, (minZ + maxZ) / 2);
        Vector3 bottomSize = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
        Gizmos.DrawCube(bottomCenter, bottomSize);
        
        // Draw top rectangle
        Vector3 topCenter = new Vector3((minX + maxX) / 2, maxY, (minZ + maxZ) / 2);
        Vector3 topSize = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
        Gizmos.DrawCube(topCenter, topSize);
        
        // Draw corner lines connecting top and bottom
        Gizmos.DrawLine(new Vector3(minX, minY, minZ), new Vector3(minX, maxY, minZ));
        Gizmos.DrawLine(new Vector3(maxX, minY, minZ), new Vector3(maxX, maxY, minZ));
        Gizmos.DrawLine(new Vector3(minX, minY, maxZ), new Vector3(minX, maxY, maxZ));
        Gizmos.DrawLine(new Vector3(maxX, minY, maxZ), new Vector3(maxX, maxY, maxZ));
    }
}