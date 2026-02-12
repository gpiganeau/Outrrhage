using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSkillData", menuName = "Scriptable Objects/Game/SkillData")]
public class SkillData : ScriptableObject
{

    [Header("Core Settings")]
    public GameObject SkillStrategyPrefab;
    public GameObject[] SkillProjectilePrefab;
    public string Name;
    public float Cooldown;

    [Header("Hold and Release")]
    public bool IsHold;
    public float HoldDuration;

    [Header("Projectile Settings")]
    [Range (0, 32)] public float Radius = 0;
    [Range (0, 50)] public float ProjectileRange;
    public float ProjectileSpeed;
    public int[] ProjectileDamage;
    public float ProjectileLifetime;
    public int numberOfProjectiles;
    public SkillshotProjectile.TravelMode TravelMode = SkillshotProjectile.TravelMode.AwayFromCaster;
    [Range(0, 1)] public float AimAssistRatio = 0;

    [Header("Movement")]
    public bool ignoreCollisions;
    public float movementDistance;
    public float movementDuration;

    [Header("Riel")]
    public bool IsRielSpecificSkill = false;
    public int BloodCost = 0;

    [Header("Blood Stacking")]
    public int BloodStackingAmount = 1;

    [Header ("Visuals")]
    public Sprite Icon;
    public string AnimationKey = "Slash";
    public SkillVFXData vfxData;
}
