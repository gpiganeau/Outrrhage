using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using DG.Tweening;
using Unity.Properties;

public abstract class Projectile: MonoBehaviour
{
    
    protected ProjectileData _data; 
    public Team Team => _data.Team;
    [HideInInspector] public UnityEvent<Projectile> onProjectileRemoval;
    [HideInInspector] public UnityEvent<Projectile, DamageController> onProjectileHit;
    public abstract void Initialize(ProjectileData data);
	

    protected void DestroyProjectile()
    {
        DOTween.Kill(this);
        onProjectileRemoval?.Invoke(this);
    }
}
