using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using DG.Tweening;
using Unity.Properties;

public abstract class Projectile: MonoBehaviour
{
    protected ProjectileData _data;
    [HideInInspector] public UnityEvent<Projectile> onProjectileRemoval;
    [HideInInspector] public UnityEvent<Projectile> onProjectileHit;
    public abstract void Initialize(ProjectileData data);
	

    protected void DestroyProjectile()
    {
        DOTween.Kill(this);
        onProjectileRemoval?.Invoke(this);
    }
}
