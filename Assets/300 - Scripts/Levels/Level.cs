using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] RespawnPoint _spawnPoint;
    public RespawnPoint GetSpawnPoint() => _spawnPoint;
}
