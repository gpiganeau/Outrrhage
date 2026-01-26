using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStrategy : MonoBehaviour
{
    protected SkillsController parentController;
    private string debugName;
    protected bool isInCooldown = false;
    protected List<Projectile> activeProjectiles;

    protected SkillData _storedSkillData;
    public SkillData SkillData => _storedSkillData;

    public virtual void Initialize(SkillsController parent, SkillData skillData)
    {
        parentController = parent;
        debugName = skillData.name;
        activeProjectiles = new List<Projectile>();
        _storedSkillData = skillData;
    }

    public virtual void Call(MovementController movementController)
    {
        if (isInCooldown) return;
        Debug.Log($"Skill {debugName} used");
    }

    protected Projectile SpawnProjectile(ProjectileData data)
    {
        Projectile newProjectile = Instantiate(_storedSkillData.SkillProjectilePrefab.gameObject).GetComponent<Projectile>();
        newProjectile.Initialize(data);
        activeProjectiles.Add(newProjectile);
        newProjectile.onProjectileRemoval.AddListener(RemoveProjectile);
        return newProjectile;
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
        DOVirtual.DelayedCall(_storedSkillData.Cooldown, () => isInCooldown = false);
    }
}
