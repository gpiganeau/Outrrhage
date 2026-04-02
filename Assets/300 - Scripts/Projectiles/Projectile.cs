using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public abstract class Projectile: MonoBehaviour
{
    
    protected ProjectileData _data; 
    public ProjectileData Data => _data;
    [HideInInspector] public UnityEvent<Projectile> onProjectileRemoval;
    [HideInInspector] public UnityEvent<Projectile, DamageController> onProjectileHit;
    public abstract void Initialize(ProjectileData data);
	public Transform casterTransform;

    protected void DestroyProjectile()
    {
        DOTween.Kill(this);
        onProjectileRemoval?.Invoke(this);
    }
    
    public void ForceExpire()
    {
        DestroyProjectile();
    }
}
