using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StaticProjectileOnCaster: Projectile
{
    private int _damage = 1;
    private Transform _casterTransform;
    private Vector3 _offset;

    public override void Initialize(ProjectileData data)
    {
        _data = data;
        _casterTransform = data.casterTransform;
        _offset = Vector3.zero;

        transform.position = data.startingPosition;
        Vector3 originToProj = transform.position - data.origin;
        transform.forward = originToProj.normalized;
        _damage = data.Damage;
        DOVirtual.DelayedCall(data.Lifetime, DestroyProjectile);
    }

    private void Update()
    {
        if (_casterTransform != null)
        {
            transform.position = _casterTransform.position + _offset;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageController damageController = other.GetComponent<DamageController>();
        if(damageController != null)
        {
            if (_data.useCustomDamageCalculation)
            {
                _damage = _data.attackStrategy.CustomDamageCalculation(damageController, _damage, this);
            }
            
            damageController.Damage(_damage, transform.position, _data.Team);
            onProjectileHit?.Invoke(this, damageController);
        }
    }

    //The stuff that will make the projectile go and move 
    //Probably need to set a timer to destroy it after some time
    //Or on collision with something, that kinda stuff

    //On voudra peut-être éviter l'effet shotgun avec les attaques qui tirent plusieurs projectiles en même temps
    //Pour ça aura besoin de savoir quelle instance de tir a créé le projectile pour lock les dégats après la première instance

    //Le projectile gère les collisions et les dégats qu'il inflige
    //L'idée c'est qu'il soit fire and forget

}
