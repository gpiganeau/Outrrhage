using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using DG.Tweening;

public class Projectile: MonoBehaviour
{
    [HideInInspector] public UnityEvent<Projectile> onProjectileRemoval;
	public void Initialize(ProjectileData data)
	{
        transform.position = data.startingPosition;
        DOVirtual.DelayedCall(1, DestroyProjectile);
        //The stuff that will make the projectile go and move 
        //Probably need to set a timer to destroy it after some time
        //Or on collision with something, that kinda stuff

        //On voudra peut-être éviter l'effet shotgun avec les attaques qui tirent plusieurs projectiles en même temps
        //Pour ça aura besoin de savoir quelle instance de tir a créé le projectile pour lock les dégats après la première instance

        //Le projectile gère les collisions et les dégats qu'il inflige
        //L'idée c'est qu'il soit fire and forget
    }

    private void DestroyProjectile()
    {
        onProjectileRemoval?.Invoke(this);
    }

    // Use this for initialization
    void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
