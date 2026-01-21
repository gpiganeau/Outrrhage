using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using DG.Tweening;

public abstract class Projectile: MonoBehaviour
{
    protected ProjectileData _data;
    [HideInInspector] public UnityEvent<Projectile> onProjectileRemoval;
    public abstract void Initialize(ProjectileData data);
	

    protected void DestroyProjectile()
    {
        onProjectileRemoval?.Invoke(this);
    }
}
