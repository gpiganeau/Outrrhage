using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BarkTrigger : MonoBehaviour
{
    [SerializeField] Bark _bark;

    SphereCollider _collider;
    [SerializeField, Range(2f, 10f)] float _radius = 3.0f;


    void Start()
    {
        if (_collider == null) _collider =  GetComponent<SphereCollider>();
        _collider.radius = _radius;
    }

    void OnValidate()
    {
        if (_collider == null) _collider =  GetComponent<SphereCollider>();
        _collider.radius = _radius;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    void OnTriggerEnter(Collider other)
    {
        if (BarkManager.I.TryPlay(_bark) && _bark.OneShot)
        {
            _collider.enabled = false;
        }
    }
}
