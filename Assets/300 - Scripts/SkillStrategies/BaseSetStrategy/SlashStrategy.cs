using DG.Tweening;
using UnityEngine;

public class SlashStrategy: SkillStrategy
{
    private bool hasHitATarget = false;
    public BloodDrop bloopDropPrefab;

    MovementController cachedController;

    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) return false;

        hasHitATarget = false;
        cachedController = movementController;


        movementController.SetImmobilized(true, "SlashAttack");
        parentController.SetSkillsDisabled(true, "SlashAttack");

        Vector3 dashTarget = movementController.transform.position + movementController.GetFacingDirection() * 1.5f;
        //movementController.transform.DOMove(dashTarget, 0.15f).SetEase(Ease.OutQuad);
        movementController.transform.DOMove(dashTarget, 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            ProjectileData projectileData = new ProjectileData()
        {
            startingPosition = movementController.transform.position + 2f * movementController.GetFacingDirection(),
            origin = movementController.transform.position,
            Damage = _storedSkillData.ProjectileDamage,
            Lifetime = _storedSkillData.ProjectileLifetime,
            Team = team,
        };

        SpawnProjectile(projectileData);

        DOVirtual.DelayedCall(_storedSkillData.ProjectileLifetime, PostLifetimeEffects);
        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseMinTimeBetweenSkills, () =>
        {
            parentController.SetSkillsDisabled(false, "SlashAttack");
        });
        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseStaticTimeOnSkillUse, () =>
        {
            movementController.SetImmobilized(false, "SlashAttack");
        });
        PutInCooldown();
        });



        return true;

    }

    protected override void OnProjectileHit(Projectile projectile)
    {
        base.OnProjectileHit(projectile);
        hasHitATarget = true;

    }

    private void PostLifetimeEffects()
    {
        if (!hasHitATarget)
        {
            //Spawn bloodlet
            var pos = cachedController.transform.position;
            pos -= cachedController.transform.forward * 2.5f;
            pos = pos.WithY(1);
            Instantiate(bloopDropPrefab, pos, Quaternion.identity);
        }
    }
}
