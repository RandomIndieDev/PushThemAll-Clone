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

    
    [Header("Settings")] 
    [SerializeField] private int spawnAmount;

    [SerializeField] private float ySpawnPosition;


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

    private void SpawnEnemiesInASquare()
    {
        
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
                break;
            case 3:
                break;
            
        }
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
