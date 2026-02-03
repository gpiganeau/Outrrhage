using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public enum RespawnType { LevelStart, Checkpoint }

    public RespawnType respawnType = RespawnType.Checkpoint;
}
