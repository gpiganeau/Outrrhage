using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStrategy : MonoBehaviour
{
    protected SkillsController parentController;
    private string debugName;
    [SerializeField] protected Projectile projectilePrefab;
    private float cooldownTime;
    protected bool isInCooldown = false;
    protected List<Projectile> activeProjectiles;

    protected ProjectileData projectileData;

    public virtual void Initialize(SkillsController parent, SkillData skillData)
    {
        parentController = parent;
        debugName = skillData.name;
        cooldownTime = skillData.Cooldown;
        projectileData = new ProjectileData(skillData.ProjectileSpeed, skillData.ProjectileDamage);
        activeProjectiles = new List<Projectile>();
    }

    public virtual void Call(MovementController movementController)
    {
        if (isInCooldown) return;
        Debug.Log($"Skill {debugName} used");
    }

    protected void SpawnProjectile(Vector3 position)
    {
        Projectile newProjectile = Instantiate(projectilePrefab.gameObject).GetComponent<Projectile>();
        newProjectile.Initialize(projectileData);
        newProjectile.transform.position = position;
        activeProjectiles.Add(newProjectile);
        newProjectile.onProjectileRemoval.AddListener(RemoveProjectile);
    }

    private void RemoveProjectile(Projectile projectile)
    {
        if (activeProjectiles.Contains(projectile))
        {
            activeProjectiles.Remove(projectile);
            Destroy(projectile.gameObject);
        }
        else
        {
            Debug.LogWarning("Tried to remove a projectile that is no longer in the active list.");
        }
    }

    public void PutInCooldown()
    {
        isInCooldown = true;
        DOVirtual.DelayedCall(cooldownTime, () => isInCooldown = false);
    }
}
