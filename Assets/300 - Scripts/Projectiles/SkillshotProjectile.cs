using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SkillshotProjectile: Projectile
{

    public enum TravelMode { AwayFromCaster, TowardCaster, TowardTarget }
    TravelMode _travelMode = TravelMode.AwayFromCaster;
    public void SetTravelMode (TravelMode mode) => _travelMode = mode;

    protected int _damage = 1;
    protected Vector3 target;

    public override void Initialize(ProjectileData data)
    {
        _data = data;
        transform.position = data.startingPosition;
        Vector3 originToProj = transform.position - data.origin;
        transform.forward = originToProj.normalized;
        _damage = data.Damage;
        DOVirtual.DelayedCall(data.Lifetime, DestroyProjectile);
        target = data.Target;
    }

    void Update()
    {
        var delta = transform.forward * Time.deltaTime * _data.Speed;
        delta.y = 0;
        switch (_travelMode)
        {
            case TravelMode.AwayFromCaster:
                transform.position += delta;
                break;
            case TravelMode.TowardCaster:
                transform.position -= delta;
                break;
            case TravelMode.TowardTarget:
                var dir = (target - _data.startingPosition).normalized;
                transform.position += dir * Time.deltaTime * _data.Speed;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out DamageController dc)){
            if(dc.Damage(_damage, transform.position, _data.Team))
            {
                onProjectileHit?.Invoke(this, dc);
                DestroyProjectile();
            }
        }
    }
}
