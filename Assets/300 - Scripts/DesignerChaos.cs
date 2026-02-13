using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChaosStep
{
    public string stepName;
    [Range(0f, 10f)] public float delay;
    public UnityEvent stepEvent;
}


[RequireComponent(typeof(SphereCollider))]
public class DesignerChaos: MonoBehaviour
{
    [TextArea] public string _eventDescription;
    [Header("Settings")]
    public Color _gizmoColor = Color.yellow;
    public Color _spawnGizoColor = Color.red;
    [Range(1, 16)] public float _eventRadius;
    [Range(1, 32)] public float _spawnRadius;

    [Header("Sequence")]
    public bool useSequence = false;
    public List<ChaosStep> ChaosSequence = new List<ChaosStep>();
    
    [Header("Single Event (if not using sequence)")]
    public UnityEvent ChaosSingleEvent;
    
    private SphereCollider _collider;
    private bool _hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;
        _hasTriggered = true;
        
        if (useSequence && ChaosSequence.Count > 0)
        {
            StartCoroutine(PlaySequence());
        }
        else
        {
            ChaosSingleEvent?.Invoke();
        }
    }
     
    IEnumerator PlaySequence()
    {
        foreach (var step in ChaosSequence)
        {
            if (step.delay > 0)
            {
                yield return new WaitForSeconds(step.delay);
            }
            
            step.stepEvent?.Invoke();
        }
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

    public void KillAllEnemies()
    {
        foreach (var bot in EntityManager.Instance.Bots)
        {
            bot.ForceKill();
        }
    }

}
