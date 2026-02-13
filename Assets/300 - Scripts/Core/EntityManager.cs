using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;

    public CharacterComponent Riel;             // -- Set by Game Manager
    public List<AIActorComponent> Bots = new();

    public enum EntityType
    {
        Humanoid,
        Drones,
        Hybrid
    }


    [Header("Spawn Settings")]
    public GameObject SpawnFXPrefab;
    public List<AIActorComponent> enemyPrefabs;              
    public float spawnInterval = 2f;                
    public float spawnRangeMin = 5f;           
    public float spawnRangeMax = 10f;          
    private Sequence spawnSequence;
    public bool AutoSpawn = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    void Start()
    {
        StartInfiniteSpawning();
    }

    private void StartInfiniteSpawning()
    {
        spawnSequence = DOTween.Sequence();
        spawnSequence.AppendInterval(spawnInterval);
        spawnSequence.AppendCallback(SpawnEnemyAtRandomPosAutoSpawn);
        spawnSequence.SetLoops(-1); 
    }

    public void StopInfiniteSpawning()
    {
        spawnSequence?.Kill();
    }

    private void SpawnEnemyAtRandomPosAutoSpawn()
    {
        if (!AutoSpawn) return;
        var actor  = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        var pos = GetRandomPosAroundRiel();

        // -- Sequence for spawn FX and Enemy Spawn
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            var fx = Instantiate(SpawnFXPrefab, pos, Quaternion.identity);
        });
        seq.AppendInterval(1f); 
        seq.AppendCallback(() =>{
             var newBot = Instantiate(actor, pos, Quaternion.identity);
             Bots.Add(newBot);
        });
    }

    Vector3 GetRandomPosAroundRiel()
    {
        float randomDistance = Random.Range(spawnRangeMin, spawnRangeMax);
        Vector2 randomDirection = Random.insideUnitCircle.normalized * randomDistance;
        return Riel.transform.position + new Vector3(randomDirection.x, .5f, randomDirection.y);
    }

    private void OnDestroy()
    {
        spawnSequence?.Kill();
    }

    internal void SpawnEntities(EntityType type, int count, Vector3 position, float spawnRadius)
    {
        
    }
}

