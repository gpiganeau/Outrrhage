using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class BloodExplosion : SkillStrategy
{
    Sequence channelSequence;
    Team storedTeam;
    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) return false;

        storedTeam = team;
        movementController.SetImmobilized(true, "SlashAttack");
        parentController.SetSkillsDisabled(true, "SlashAttack");

        channelSequence = DOTween.Sequence();
        channelSequence.AppendInterval(_storedSkillData.HoldDuration);
        channelSequence.OnComplete(() =>
        {
            ExecuteExplosion();
            parentController.SetSkillsDisabled(false, "SlashAttack");
            movementController.SetImmobilized(false, "SlashAttack");
        });        

        return true;
    }

    private void ExecuteExplosion()
    {
        List<BloodDrop> nearbyBloodDrops = new List<BloodDrop>();
        Collider[] bloodDropColliders = Physics.OverlapSphere(transform.position, _storedSkillData.Radius, LayerMask.GetMask("Blood"));
        foreach(Collider drop in bloodDropColliders)
        {
            nearbyBloodDrops.Add(drop.GetComponent<BloodDrop>());
        }

        foreach(BloodDrop drop in nearbyBloodDrops)
        {
            ProjectileData projectileData = new ProjectileData()
            {
                startingPosition = drop.transform.position,
                origin = drop.transform.position,
                Damage = _storedSkillData.ProjectileDamage,
                Lifetime = _storedSkillData.ProjectileLifetime,
                Team = storedTeam,
            };
            SpawnProjectile(projectileData);
            Destroy(drop.gameObject);
        }
    }

    public override void Release(MovementController movementController, Team team)
    {
        base.Release(movementController, team);

        if (channelSequence != null) 
        {
            channelSequence.Kill();
            parentController.SetSkillsDisabled(false, "SlashAttack");
            movementController.SetImmobilized(false, "SlashAttack");
        }
    }
}

