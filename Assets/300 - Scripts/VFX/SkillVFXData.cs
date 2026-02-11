using System;
using UnityEngine;

[Serializable]
public class SkillVFXData
{
    [Header("Casting VFX")]
    public GameObject castVFXPrefab;
    public Vector3 castVFXOffset;
    public bool attachCastVFXToCharacter = true;
    
    [Header("Projectile VFX")]
    public GameObject projectileTrailVFXPrefab;
    public GameObject projectileImpactVFXPrefab;
    
    [Header("Area VFX")]
    public GameObject areaIndicatorVFXPrefab;
    
    [Header("Hit VFX")]
    public GameObject hitVFXPrefab;
    
    [Header("Sound")]
    public AudioClip castSound;
    public AudioClip projectileSound;
    public AudioClip impactSound;
}