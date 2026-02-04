using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;

    public CharacterComponent Riel;             // -- Set by Game Manager
    public List<AIActorComponent> Bots = new();


    [Header("Spawn Settings")]
    public AIActorComponent enemyPrefab;              
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
        spawnSequence.SetLoops(-1); // -1 = infinite loops
    }

    private void SpawnEnemyAtRandomPosAutoSpawn()
    {
        if (!AutoSpawn) return;
        Bots.Add(Instantiate(enemyPrefab, GetRandomPosAroundRiel(), Quaternion.identity));
    }

    Vector3 GetRandomPosAroundRiel()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(spawnRangeMin, spawnRangeMax);
        return Riel.transform.position + new Vector3(randomDirection.x, .5f, randomDirection.y) * randomDistance;
    }

    private void OnDestroy()
    {
        spawnSequence?.Kill();
    }
}

