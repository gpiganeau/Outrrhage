using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class RespawnPoint : MonoBehaviour
{
    public enum RespawnType { LevelStart, Checkpoint }

    public RespawnType respawnType = RespawnType.Checkpoint;

    [SerializeField] private float _safeRadius = 5f;
    private SphereCollider _collider;
    private ParticleSystem _particleSystem;

    [SerializeField] private bool _safeZone = false;

    [Header("Custom for HUB")]
    public bool HasStartRun = false;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _collider.radius = _safeRadius;
    }
    public void OnTriggerStay(Collider other)
    {

        if (!_safeZone) return;
        
        if (other.TryGetComponent<CharacterComponent>(out var character))
        {
            CharacterComponent.Blood.Regain(100);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!HasStartRun)
        {
            HasStartRun = true;
            RunSystemController.Instance.QueryRoomEnd(true);
        }
    }

    void OnValidate()
    {
        if (_collider == null) _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _collider.radius = _safeRadius;
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        if (_particleSystem != null)
        {
            var shapeModule = _particleSystem.shape;
            shapeModule.shapeType = ParticleSystemShapeType.Sphere;
            shapeModule.radius = _safeRadius;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _safeRadius);
    }
}
