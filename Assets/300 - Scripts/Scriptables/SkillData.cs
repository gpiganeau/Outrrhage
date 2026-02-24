using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSkillData", menuName = "Scriptable Objects/Game/SkillData")]
public class SkillData : ScriptableObject
{

    [Header("Core Settings")]
    public GameObject SkillStrategyPrefab;
    public GameObject[] SkillProjectilePrefab;
    public string Name;
    [TextArea] public string Description;
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

    [Header("Combo Settings")]
    public float ComboResetDelay;

    [Header("Misc")]
    public bool ProvideInvulnerability = false;
    public float InvulnerabilityTime = 0f;

    [Header("Movement")]
    public bool ignoreCollisions;
    public float movementDistance;
    public float movementDuration;

    [Header("Riel")]
    public bool IsRielSpecificSkill = false;
    public int BloodCost = 0;
    public bool DropBloodOnFailedSkill = false;
    public int RageGain = 1;

    [Header("Blood Stacking")]
    public int BloodStackingAmount = 1;

    [Header ("Visuals")]
    public Sprite Icon;
    public string AnimationKey = "Slash";
    [Tooltip("Use in cases of multiples moves")] public string[] AnimationsKeys;
    public SkillVFXData vfxData;

    [Header("Audio")]
    public AudioClip[] castClips;
    public AudioClip[] hitClips;
}
