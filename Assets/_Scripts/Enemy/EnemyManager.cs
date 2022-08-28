using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    [Header("Target")] 
    [SerializeField] private GameObject player;

    [Header("References")] 
    [SerializeField] private EnemyPooler enemyPooler;


    [Header("Zone One Spawn Settings")]
    [SerializeField] private Transform zoneOneCircleSpawnLoc;
    [SerializeField] private int zoneOneCircleSpawnAmt;
    [SerializeField] private float zoneOneCircleSpawnRadius;

    [Header("Zone Two Spawn Settings")] 
    [SerializeField] private Transform zoneTwoSpawnLoc;
    [SerializeField] private Vector2 xSpawnTwoPos;
    [SerializeField] private Vector2 zSpawnTwoPos;
    [SerializeField] private int zoneTwoSpawnAmt;
    
    [Header("Zone Three Spawn Settings")] 
    [SerializeField] private Transform zoneThreeSpawnLoc;
    [SerializeField] private Vector2 xSpawnThreePos;
    [SerializeField] private Vector2 zSpawnThreePos;
    [SerializeField] private int zoneThreeSpawnAmt;
    
    
    [Header("Settings")]
    [SerializeField] private float ySpawnPosition;
    [SerializeField] private int spawnCollisionCheckRadius;
    [SerializeField] private LayerMask collisionMask;

    private int currentSpawnCount;
    private int currentZone;

    void Start()
    {
        currentZone = 1;
        SpawnZoneEnemies();
    }

    void OnEnable()
    {
        EventsManager.Instance.OnSpawnTriggerEntered += SpawnZoneEnemies;
    }

    void OnDestroy()
    {
        EventsManager.Instance.OnSpawnTriggerEntered -= SpawnZoneEnemies;
    }

    private List<Enemy> SpawnEnemiesInACircle(Vector3 pos, int amountToSpawn, float radius)
    {
        
        var spawnedEnemies = new List<Enemy>();
        
        for (int i = 0; i < amountToSpawn; i++)
        {
            float angle = i * Mathf.PI*2f / amountToSpawn;
            pos.y = 0;
            Vector3 newPos = new Vector3(Mathf.Cos(angle)*radius, ySpawnPosition, Mathf.Sin(angle)*radius);
            var spawnedEnemy = enemyPooler.SpawnFromPool(newPos + pos, Quaternion.identity);

            spawnedEnemies.Add(spawnedEnemy);
        }

        return spawnedEnemies;
    }
    private void PrepEnemy(List<Enemy> spawnedEnemies)
    {
        foreach (var enemy in spawnedEnemies)
        {
            enemy.Initialize(player.transform, this);
            currentSpawnCount++;
        }
    }

    private void ZoneCompleted()
    {
        switch (currentZone)
        {
            case 1:
                EventsManager.Instance.ZoneOneCompleted();
                break;
            case 2:
                EventsManager.Instance.ZoneTwoCompleted();
                break;
            case 3:
                EventsManager.Instance.ZoneThreeCompleted();
                break;
        }

        currentZone++;
    }

    private void SpawnZoneEnemies()
    {
        switch (currentZone)
        {
            case 1:
                
                PrepEnemy(SpawnEnemiesInACircle(zoneOneCircleSpawnLoc.position, zoneOneCircleSpawnAmt, zoneOneCircleSpawnRadius));
                
                break;
            case 2:
                
                PrepEnemy(SpawnEnemiesInZone(zoneTwoSpawnLoc.position, xSpawnTwoPos, zSpawnTwoPos));
                print("Called 2");
                break;
            case 3:
                
                PrepEnemy(SpawnEnemiesInZone(zoneThreeSpawnLoc.position, xSpawnThreePos, zSpawnThreePos));
                print("Called 3");
                break;
            
        }
    }

    private List<Enemy> SpawnEnemiesInZone(Vector3 location, Vector3 xOffset, Vector3 zOffset)
    {
        var spawnedEnemies = new List<Enemy>();
        
        for (int i = 0; i < zoneTwoSpawnAmt; i++)
        {
            var xPos = Random.Range(xOffset.x, xOffset.y);
            var zPos = Random.Range(zOffset.x, zOffset.y);
            
            var spawnLocation = location + new Vector3(xPos, 0, zPos);
            spawnLocation.y = ySpawnPosition;

            if (Physics.CheckSphere(spawnLocation, spawnCollisionCheckRadius, collisionMask)) continue;
            
            var spawnedEnemy = enemyPooler.SpawnFromPool(spawnLocation, Quaternion.identity);
            
            
            
            spawnedEnemies.Add(spawnedEnemy);


        }
        
        return spawnedEnemies;
    }

    public void EnemyDisabled()
    {
        currentSpawnCount--;

        if (currentSpawnCount <= 0)
        {
            ZoneCompleted();
        }
    }


}
