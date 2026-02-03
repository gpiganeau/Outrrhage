using System;
using UnityEngine;

[Serializable]
public class SkillVFXData
{
    [Header("Casting VFX")]
    public GameObject castVFXPrefab;
    public Vector3 castVFXOffset;
    public bool attachCastVFXToCharacter = true;
    public float castVFXDuration = 1f;
    
    [Header("Projectile VFX")]
    public GameObject projectileTrailVFXPrefab;
    public GameObject projectileImpactVFXPrefab;
    public bool scaleProjectileVFXWithRadius = false;
    
    [Header("Area VFX")]
    public GameObject areaIndicatorVFXPrefab;
    public bool scaleAreaVFXWithRadius = true;
    public Color areaVFXColor = Color.white;
    
    [Header("Hit VFX")]
    public GameObject hitVFXPrefab;
    public bool scaleHitVFXWithDamage = false;
    public float hitVFXLifetime = 2f;
    
    [Header("Sound")]
    public AudioClip castSound;
    public AudioClip projectileSound;
    public AudioClip impactSound;
}