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
    protected SkillVFXController _vfxController;
    public SkillData SkillData => _storedSkillData;
    public bool IsInCooldown => isInCooldown;

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
      
        Logger.Combat($"Skill {debugName} used and consumed {_storedSkillData.BloodCost}");

        return true;
    }

    public virtual void Release(MovementController movementController, Team team)
    {
        movementController.StopAimingMode();
        // For chargeable skills, we might want to do something when the player releases the button
    }

    protected Projectile SpawnProjectile(ProjectileData data, int projectileIndex)
    {
        Projectile newProjectile = Instantiate(_storedSkillData.SkillProjectilePrefab[projectileIndex].gameObject).GetComponent<Projectile>();
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

    protected virtual void OnProjectileHit(Projectile projectile, DamageController damageController)
    {
        _vfxController.PlayHitVFX(projectile.transform.position, _storedSkillData.ProjectileDamage[0]);
    }

    public virtual int CustomDamageCalculation(DamageController target, int baseDamage, Projectile projectile)
    {
        throw new System.NotImplementedException("CustomDamageCalculation is not implemented for this skill.");
    }

    public bool UseAimAssist(ref Vector3 aimDirection, float aimAssistRatio, Team myTeam)
    {
        Vector3 originalAimDir = aimDirection;
        aimAssistRatio = Mathf.Clamp01(aimAssistRatio);
        Collider[] results = Physics.OverlapSphere(transform.position, Mathf.Max(_storedSkillData.Radius, _storedSkillData.ProjectileRange));
        List<DamageController> potentialTargets = new List<DamageController>();

        foreach(Collider collider in results)
        {
            if (collider.TryGetComponent(out DamageController targetDamageController))
            {
                Vector3 targetDirection = targetDamageController.transform.position - transform.position;
                if (Mathf.Abs(Quaternion.FromToRotation(aimDirection, targetDirection).eulerAngles.y) / 180 <= aimAssistRatio)
                {
                    potentialTargets.Add(targetDamageController);
                }
            }
        }

        if(potentialTargets.Count == 0)
        {
            return false;
        }

        potentialTargets.Sort((a, b) => 
        {
            Vector3 aDirection = a.transform.position - transform.position;
            Vector3 bDirection = b.transform.position - transform.position;
            float aAngle = Mathf.Abs(Quaternion.FromToRotation(originalAimDir, aDirection).eulerAngles.y);
            float bAngle = Mathf.Abs(Quaternion.FromToRotation(originalAimDir, bDirection).eulerAngles.y);
            return aAngle.CompareTo(bAngle);
        });


        List<DamageController> validAllyTargets = new List<DamageController>();
        List<DamageController> validEnemyTargets = new List<DamageController>();
        List<DamageController> validNeutralTargets = new List<DamageController>();

        foreach(DamageController target in potentialTargets)
        {
            if(target.Team == Team.Ally)
            {
                validAllyTargets.Add(target);
            }
            else if(target.Team == Team.Enemy)
            {
                validEnemyTargets.Add(target);
            }
            else if(target.Team == Team.Neutral)
            {
                validNeutralTargets.Add(target);
            }
        }

        switch (myTeam)
        {
            case Team.Ally:
                if(validEnemyTargets.Count > 0)
                {
                    aimDirection = (validEnemyTargets[0].transform.position - transform.position).normalized;
                    return true;
                }
                else if (validNeutralTargets.Count > 0)
                {
                    aimDirection = (validNeutralTargets[0].transform.position - transform.position).normalized;
                    return true;
                }
                break;
            case Team.Enemy:
                if(validAllyTargets.Count > 0)
                {
                    aimDirection = (validAllyTargets[0].transform.position - transform.position).normalized;
                    return true;
                }
                if (validNeutralTargets.Count > 0)
                {
                    aimDirection = (validNeutralTargets[0].transform.position - transform.position).normalized;
                    return true;
                }
                break;
            case Team.Neutral:
                if(potentialTargets.Count > 0)
                {
                    aimDirection = (potentialTargets[0].transform.position - transform.position).normalized;
                    return true;
                }
                break;
        }

        return false;
    }

    protected void StackBlood(Projectile projectile, DamageController damageController)
    {
        if (damageController.GetComponent<BloodStack>() != null)
        {
            damageController.GetComponent<BloodStack>().Increase(projectile.Data.BloodStackingAmount);
        }
    }
}
