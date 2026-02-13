using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class DesignerChaos: MonoBehaviour
{
    [TextArea] public string _eventDescription;
    [Header("Settings")]
    public Color _gizmoColor = Color.yellow;
    public Color _spawnGizoColor = Color.red;
    [Range(1, 16)] public float _eventRadius;
    [Range(1, 32)] public float _spawnRadius;

    [Header("Event")]
    public UnityEvent ChaosEvent;

    private SphereCollider _collider;
    void OnTriggerEnter(Collider other)
    {
        ChaosEvent?.Invoke();
    }

    void OnValidate()
    {
        if (_collider == null) _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _collider.radius = _eventRadius;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireSphere(transform.position, _collider.radius);

        Gizmos.color = _spawnGizoColor;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }

    public void SpawnDrones(int count)
    {
        EntityManager.Instance.SpawnEntities(EntityManager.EntityType.Drones, count, transform.position, _spawnRadius);
    }

    public void SpawnHumans(int count)
    {
        EntityManager.Instance.SpawnEntities(EntityManager.EntityType.Humanoid, count, transform.position, _spawnRadius);
    }

    public void ChangeCameraSetting(CameraSettings cameraSettings)
    {
        GameManager.Instance.CameraController.SetCameraSettings(cameraSettings);
    }

    public void ResetCameraSetting()
    {
        GameManager.Instance.CameraController.ResetCameraSettings();
    }

}
