using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public virtual bool Call(MovementController movementController)
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

        Blood b = CharacterComponent.Blood;

        if (_storedSkillData.IsRielSpecificSkill && b.Amount < _storedSkillData.BloodCost){
            
            Logger.LogError(Logger.LogCategory.Combat, "Can't Perform Skill: no more blood");
            return false; // Todo : Feedback
        }

        b.Consume(_storedSkillData.BloodCost);
        Logger.Combat($"Skill {debugName} used and consumed {_storedSkillData.BloodCost}. Blood Remaining : {b.Amount}");

        return true;
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
            Logger.LogWarning(Logger.LogCategory.Combat, "Tried to remove a projectile that is no longer in the active list.");
        }
    }

    public void PutInCooldown()
    {
        isInCooldown = true;
        DOVirtual.DelayedCall(_storedSkillData.Cooldown, () => isInCooldown = false);
    }
}
