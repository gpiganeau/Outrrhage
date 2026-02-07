using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class SkillStrategy : MonoBehaviour
{
    protected SkillsController parentController;
    private string debugName;
    protected bool isInCooldown = false;
    protected List<Projectile> activeProjectiles;

    protected SkillData _storedSkillData;
    protected SkillVFXController _vfxController;
    public SkillData SkillData => _storedSkillData;

    public virtual void Initialize(SkillsController parent, SkillData skillData)
    {
        parentController = parent;
        debugName = skillData.name;
        activeProjectiles = new List<Projectile>();
        _storedSkillData = skillData;

        // -- VFX Controller
        GameObject obj = new GameObject($"{skillData.Name}_VFXController");
        obj.transform.parent = this.transform;
        _vfxController = obj.AddComponent<SkillVFXController>();
        _vfxController.Initialize(skillData);
    }

    public virtual bool Call(MovementController movementController, Team team)
    {
        if(isInCooldown)
        {
            //Logger.Combat("Skill is in cooldown.");
            return false;
        }
        if (movementController == null)
        {
            Logger.LogError(Logger.LogCategory.Combat, "MovementController is null.");
            return false;
        }

        CharacterComponent.Blood.Consume(_storedSkillData.BloodCost);
      
        //Logger.Combat($"Skill {debugName} used and consumed {_storedSkillData.BloodCost}. Blood Remaining : {b.Amount}");

        return true;
    }

    protected Projectile SpawnProjectile(ProjectileData data)
    {
        Projectile newProjectile = Instantiate(_storedSkillData.SkillProjectilePrefab.gameObject).GetComponent<Projectile>();
        newProjectile.Initialize(data);
        newProjectile.onProjectileHit.AddListener(OnProjectileHit);
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
            Logger.LogWarning(Logger.LogCategory.Combat, "Tried to remove a projectile that is no longer in the active list.");
        }
    }

    public void PutInCooldown()
    {
        isInCooldown = true;
        DOVirtual.DelayedCall(_storedSkillData.Cooldown, () => isInCooldown = false);
    }

    virtual protected void OnProjectileHit(Projectile projectile)
    {
        _vfxController.PlayHitVFX(projectile.transform.position, _storedSkillData.ProjectileDamage);
    }

}
