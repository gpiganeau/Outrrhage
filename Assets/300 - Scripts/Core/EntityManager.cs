using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.Events;


public enum EntityType
{
    Humanoid,
    Drones,
    Bull,
    Tourelle
}

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;

    public CharacterComponent Riel;             // -- Set by Game Manager
    public List<AIActorComponent> Bots = new();


    [Header("Spawn Settings")]
    public GameObject SpawnFXPrefab;
    public List<AIActorComponent> enemyPrefabs;              
    public float spawnInterval = 2f;                
    public float spawnRangeMin = 5f;           
    public float spawnRangeMax = 10f;          
    private Sequence spawnSequence;
    public bool AutoSpawn = false;

    public UnityEvent<EntityType> OnEnemyDied;
    public UnityEvent<EntityType> OnEnemySpawned;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public void NotifyDeath(AIActorComponent bot)
    {
        Bots.Remove(bot);
        if (OnEnemyDied != null)
        {
            OnEnemyDied?.Invoke(bot.EntityType);
        }
    }

    public void SpawnCustomActor(AIActorComponent actor, float radius)
    {
        SpawnEnemy(actor, GetRandomPosAroundRiel(radius));
    }
    private void SpawnEnemy(AIActorComponent actor, Vector3 position)
    {
        if (GameManager.GameOver) return;

        // -- Sequence for spawn FX and Enemy Spawn
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            var fx = Instantiate(SpawnFXPrefab, position, Quaternion.identity);
        });
        seq.AppendInterval(1f); 
        seq.AppendCallback(() =>{
             var newBot = Instantiate(actor, position, Quaternion.identity);
             Bots.Add(newBot);
        });
    }


    Vector3 GetRandomPosAroundRiel(float radius)
    {
        return GetRandomPosAroundPoint(Riel.transform.position, radius * 0.5f, radius);
    }
    Vector3 GetRandomPosAroundRiel()
    {
        return GetRandomPosAroundPoint(Riel.transform.position, spawnRangeMin, spawnRangeMax);
    }

    Vector3 GetRandomPosAroundPoint(Vector3 point, float minRange, float maxRange)
    {
        float randomDistance = Random.Range(minRange, maxRange);
        Vector2 randomDirection = Random.insideUnitCircle.normalized * randomDistance;
        return point + new Vector3(randomDirection.x, 0f, randomDirection.y);
    }

    private void OnDestroy()
    {
        spawnSequence?.Kill();
    }

    public void FullClearRoom()
    {
        // -- reverse loop because we change entity
        for (int i = Bots.Count -1; i >= 0; i--)
        {
            Bots[i].ForceKill();
        }
    }


    internal void SpawnEntities(EntityType type, int count, Vector3 position, float spawnRadius)
    {
        Sequence spawnSequence = DOTween.Sequence();

        var s = SettingsManager.Instance.GameplaySettings;
        float delayBetweenSpawns = s.spawnerTimeBetweenSpawns;

         // -- Sort potential spawn positions by angle to player, to make sure the first spawned enemies are the ones in front of the playe
        List<Vector3> potentialSpawnPositions = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = GetRandomPosAroundPoint(position, spawnRadius * 0.5f, spawnRadius);
            potentialSpawnPositions.Add(spawnPos);
        }

         Vector3 originalAimDir = (Riel.transform.position - position).normalized;

         potentialSpawnPositions.Sort((a, b) => 
        {
            Vector3 aDirection = a - position;
            Vector3 bDirection = b - position;
            float aAngle = Mathf.Abs(Quaternion.FromToRotation(originalAimDir, aDirection).eulerAngles.y);
            float bAngle = Mathf.Abs(Quaternion.FromToRotation(originalAimDir, bDirection).eulerAngles.y);
            return aAngle.CompareTo(bAngle);
        });

        
        for (int i = 0; i < count; i++)
        {   
            spawnSequence.AppendCallback(() =>
            {
                Vector3 spawnPos = GetRandomPosAroundPoint(position, spawnRadius * 0.5f, spawnRadius);
                //Vector3 spawnPos = potentialSpawnPositions[i];

              //  Logger.Combat($"Spawning {type} at {spawnPos}");
                
                AIActorComponent prefabToSpawn = type switch
                {
                    EntityType.Drones => enemyPrefabs[0],
                    EntityType.Humanoid => enemyPrefabs[1],
                    EntityType.Bull => enemyPrefabs[2],
                    EntityType.Tourelle => enemyPrefabs[3],
                    _ => null
                };
                
                if (prefabToSpawn != null)
                {
                    SpawnEnemy(prefabToSpawn, spawnPos.WithY(SettingsManager.Instance.GameplaySettings.YSpawnOffset));
                }
            });
            
            if (i < count - 1 && delayBetweenSpawns > 0f)
            {
                spawnSequence.AppendInterval(delayBetweenSpawns);
            }
        }
    }
}

