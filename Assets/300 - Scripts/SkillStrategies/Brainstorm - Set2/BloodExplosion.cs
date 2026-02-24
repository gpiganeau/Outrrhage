using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

class BloodExplosion : SkillStrategy
{
    Vector3 storedAimPosition;
    Sequence channelSequence;
    Team storedTeam;
    [SerializeField] float _capsuleHeight = 1f;
    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) return false;

        storedTeam = team;
        movementController.SetImmobilized(true, "SlashAttack");
        parentController.SetSkillsDisabled(true, "SlashAttack");
		movementController.AnimController?.Trigger(_storedSkillData.AnimationsKeys[0]);


        channelSequence = DOTween.Sequence();
        channelSequence.AppendInterval(_storedSkillData.HoldDuration);
        channelSequence.OnComplete(() =>
        {
            storedAimPosition = movementController.GetAimPosition();
            Debug.Log($"AimPosition: {storedAimPosition} | PlayerPosition: {transform.position}");
            ExecuteExplosion();
            parentController.SetSkillsDisabled(false, "SlashAttack");
            movementController.SetImmobilized(false, "SlashAttack");
		    movementController.AnimController?.Trigger(_storedSkillData.AnimationsKeys[1]);
        });        

        movementController.StartAimingMode(new PreviewData
        {
            shapeType = ShapeType.Area,
            deployType = DeployType.Free,
            radius = _storedSkillData.Radius,
            range = _storedSkillData.ProjectileRange,
            timeToDeploy = _storedSkillData.HoldDuration,
        });

        return true;
    }

    private void ExecuteExplosion()
    {
        List<BloodDrop> nearbyBloodDrops = new List<BloodDrop>();
        Vector3 bottom = new Vector3(storedAimPosition.x, storedAimPosition.y - _capsuleHeight, storedAimPosition.z);
        Vector3 top = new Vector3(storedAimPosition.x, storedAimPosition.y + _capsuleHeight, storedAimPosition.z);
        Collider[] bloodDropColliders = Physics.OverlapCapsule(bottom, top, _storedSkillData.Radius, LayerMask.GetMask("Blood"));
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
                Damage = _storedSkillData.ProjectileDamage[0],
                Lifetime = _storedSkillData.ProjectileLifetime,
                Team = storedTeam,
                BloodStackingAmount = _storedSkillData.BloodStackingAmount,
            };
            SpawnProjectile(projectileData, 0);
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

