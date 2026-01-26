using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SlashStrategy: SkillStrategy
{
    public override bool Call(MovementController movementController)
    {
        if (isInCooldown) return false;

        ProjectileData projectileData = new ProjectileData() { 
            startingPosition = movementController.transform.position + 1.5f * movementController.GetFacingDirection(), 
            origin = movementController.transform.position,
            Damage = _storedSkillData.ProjectileDamage,
            Lifetime = _storedSkillData.ProjectileLifetime
        };

        base.Call(movementController);
        movementController.SetImmobilized(true, "SlashAttack");
        parentController.SetSkillsDisabled(true, "SlashAttack");
        SpawnProjectile(projectileData);

        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseMinTimeBetweenSkills, () =>
        {
            parentController.SetSkillsDisabled(false, "SlashAttack");
        });
        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseStaticTimeOnSkillUse, () =>
        {
            movementController.SetImmobilized(false, "SlashAttack");
        });
        PutInCooldown();
        return true;
    }

}
